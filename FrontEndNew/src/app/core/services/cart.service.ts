import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, switchMap, catchError } from 'rxjs/operators';
import { ApiService } from './api.service';
import { API_ENDPOINTS } from '../constants/api-endpoints';
import { MenuItem } from './catalog.service';

// ─── Public types ─────────────────────────────────────────────────────────────

export interface CartItem {
  menuItem: MenuItem;
  quantity: number;
  restaurantId: string;
  restaurantName: string;
  /** Backend cartItemId (set after background sync) */
  backendItemId?: string;
  /** Backend cartId this item belongs to */
  backendCartId?: string;
}

// ─── Backend response shapes ──────────────────────────────────────────────────

interface BackendCartCreatedResponse {
  cartId: string;
}

interface BackendCartResponse {
  id: string;
  restaurantId: string;
}

interface BackendCartItemResponse {
  id: string;
  cartId: string;
  quantity: number;
}

@Injectable({ providedIn: 'root' })
export class CartService {

  private cartItems = new BehaviorSubject<CartItem[]>([]);
  public cartItems$ = this.cartItems.asObservable();

  /**
   * restaurantId → backendCartId
   * Persisted in localStorage so we never recreate carts across page refreshes.
   */
  private cartIdMap = new Map<string, string>();

  /**
   * In-flight cart creation promises — prevents race conditions when multiple
   * items are added in quick succession before the first cart is created.
   */
  private pendingCartCreations = new Map<string, Promise<string>>();

  constructor(private api: ApiService) {
    this.loadFromStorage();
  }

  // ─── Storage ──────────────────────────────────────────────────────────────

  private loadFromStorage(): void {
    try {
      const cart = localStorage.getItem('cart');
      if (cart) this.cartItems.next(JSON.parse(cart));

      const map = localStorage.getItem('cart_id_map');
      if (map) this.cartIdMap = new Map(JSON.parse(map));
    } catch { /* ignore */ }
  }

  private persist(): void {
    localStorage.setItem('cart', JSON.stringify(this.cartItems.value));
    localStorage.setItem('cart_id_map', JSON.stringify([...this.cartIdMap.entries()]));
  }

  // ─── Public accessors ─────────────────────────────────────────────────────

  getCartItems(): CartItem[] { return this.cartItems.value; }
  getCartCount(): number { return this.cartItems.value.reduce((s, i) => s + i.quantity, 0); }
  getCartTotal(): number {
    return this.cartItems.value.reduce((s, i) => s + i.menuItem.price * i.quantity, 0);
  }

  // ─── Add item — OPTIMISTIC ────────────────────────────────────────────────

  /**
   * ① Updates UI immediately (no waiting).
   * ② Ensures a backend cart exists for this restaurant (creates one if needed).
   * ③ Syncs the item with the backend in background.
   *
   * Returns Observable so callers can react to backend errors if needed,
   * but the UI is already updated by the time the Observable is subscribed.
   */
  addItem(menuItem: MenuItem, restaurantId: string, restaurantName: string): Observable<any> {

    // ── Step 1: Optimistic local update ────────────────────────────────────
    const current = this.cartItems.value;
    const existing = current.find(i => i.menuItem.id === menuItem.id);

    if (existing) {
      const previousQuantity = existing.quantity;

      // Increase quantity immediately in UI
      const updated = current.map(i =>
        i.menuItem.id === menuItem.id ? { ...i, quantity: previousQuantity + 1 } : i
      );
      this.cartItems.next(updated);
      this.persist();

      // ── Step 2: Sync quantity update to backend (background) ─────────────
      return this.getOrCreateCart(restaurantId).pipe(
        switchMap(cartId => {
          if (!existing.backendItemId) return of(null); // not yet synced, skip
          return this.api.put<BackendCartItemResponse>(API_ENDPOINTS.CART.UPDATE_ITEM, {
            id: existing.backendItemId,
            quantity: previousQuantity + 1
          });
        }),
        catchError(err => {
          // Rollback: restore old quantity
          console.error('[CartService] increment failed, rolling back:', err);
          this.cartItems.next(
            this.cartItems.value.map(i =>
              i.menuItem.id === menuItem.id ? { ...i, quantity: previousQuantity } : i
            )
          );
          this.persist();
          throw err;
        })
      );

    } else {
      // New item — add to UI immediately as a temporary entry
      const tempItem: CartItem = { menuItem, quantity: 1, restaurantId, restaurantName };
      this.cartItems.next([...current, tempItem]);
      this.persist();

      // ── Step 2: Ensure cart exists then POST item (background) ───────────
      return this.getOrCreateCart(restaurantId).pipe(
        switchMap(cartId => {
          return this.api.post<BackendCartItemResponse>(API_ENDPOINTS.CART.ADD_ITEM, {
            cartId,
            menuItemId: menuItem.id,
            quantity: 1
          }).pipe(
            tap(resp => {
              // Back-fill backend IDs into the existing local item
              const items = this.cartItems.value.map(i =>
                i.menuItem.id === menuItem.id
                  ? { ...i, backendItemId: resp.id, backendCartId: cartId }
                  : i
              );
              this.cartItems.next(items);
              this.persist();
            })
          );
        }),
        catchError(err => {
          // Rollback: remove the item we optimistically added
          console.error('[CartService] add item failed, rolling back:', err);
          this.cartItems.next(this.cartItems.value.filter(i => i.menuItem.id !== menuItem.id));
          this.persist();
          throw err; // re-throw so the component can show an error
        })
      );
    }
  }

  // ─── Update quantity ──────────────────────────────────────────────────────

  updateQuantity(menuItemId: string, quantity: number): void {
    if (quantity <= 0) { this.removeItem(menuItemId); return; }

    const item = this.cartItems.value.find(i => i.menuItem.id === menuItemId);
    if (!item) return;

    const previousQuantity = item.quantity;

    // Optimistic update
    this.cartItems.next(this.cartItems.value.map(i =>
      i.menuItem.id === menuItemId ? { ...i, quantity } : i
    ));
    this.persist();

    // Background sync — rollback to previous qty on failure
    if (item.backendItemId) {
      this.api.put(API_ENDPOINTS.CART.UPDATE_ITEM, { id: item.backendItemId, quantity })
        .pipe(
          catchError(err => {
            console.error('[CartService] updateQuantity failed, rolling back:', err);
            this.cartItems.next(
              this.cartItems.value.map(i =>
                i.menuItem.id === menuItemId ? { ...i, quantity: previousQuantity } : i
              )
            );
            this.persist();
            return of(null);
          })
        )
        .subscribe();
    }
  }

  // ─── Remove item ──────────────────────────────────────────────────────────

  removeItem(menuItemId: string): void {
    const item = this.cartItems.value.find(i => i.menuItem.id === menuItemId);
    if (!item) return;

    // Snapshot current list so we can restore it at the exact same position
    const previousItems = this.cartItems.value;

    // Optimistic removal
    this.cartItems.next(previousItems.filter(i => i.menuItem.id !== menuItemId));
    this.persist();

    // Background sync — rollback (re-insert) on failure
    if (item.backendItemId && item.backendCartId) {
      this.api.delete(API_ENDPOINTS.CART.DELETE_ITEM(item.backendItemId, item.backendCartId))
        .pipe(
          catchError(err => {
            console.error('[CartService] removeItem failed, rolling back:', err);
            this.cartItems.next(previousItems); // restore full list with original order
            this.persist();
            return of(null);
          })
        )
        .subscribe();
    }
  }

  // ─── Clear ────────────────────────────────────────────────────────────────

  clearCart(): void {
    this.cartItems.next([]);
    this.cartIdMap.clear();
    this.pendingCartCreations.clear();
    localStorage.removeItem('cart');
    localStorage.removeItem('cart_id_map');
  }

  // ─── Cart provisioning ────────────────────────────────────────────────────

  /**
   * Returns the backend cartId for a restaurant.
   *
   * Fast path (common case): returns immediately from cache.
   * Slow path (first add per restaurant): creates a cart via POST /carts — NO preflight GET.
   * Race protection: if two items are added before the first cart is created,
   * they share the same in-flight promise.
   */
  private getOrCreateCart(restaurantId: string): Observable<string> {
    // Fast path — already known
    const cached = this.cartIdMap.get(restaurantId);
    if (cached) return of(cached);

    // Deduplicate concurrent cart-creation calls for the same restaurant
    const inFlight = this.pendingCartCreations.get(restaurantId);
    if (inFlight) {
      return new Observable(observer => {
        inFlight.then(id => { observer.next(id); observer.complete(); })
                .catch(err => observer.error(err));
      });
    }

    // Create a new cart (no preflight GET /active needed)
    const creation = new Promise<string>((resolve, reject) => {
      this.api.post<BackendCartCreatedResponse>(API_ENDPOINTS.CART.CREATE, { restaurantId })
        .subscribe({
          next: resp => {
            this.cartIdMap.set(restaurantId, resp.cartId);
            this.pendingCartCreations.delete(restaurantId);
            this.persist();
            resolve(resp.cartId);
          },
          error: err => {
            this.pendingCartCreations.delete(restaurantId);
            reject(err);
          }
        });
    });

    this.pendingCartCreations.set(restaurantId, creation);

    return new Observable(observer => {
      creation.then(id => { observer.next(id); observer.complete(); })
              .catch(err => observer.error(err));
    });
  }
}
