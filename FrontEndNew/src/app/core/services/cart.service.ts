import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, catchError, switchMap } from 'rxjs/operators';
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
   */
  private cartIdMap = new Map<string, string>();

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

  // ─── Add item ─────────────────────────────────────────────────────────────

  /**
   * ① Updates UI immediately (optimistic).
   * ② Syncs with backend using POST /items.
   * ③ Backend automatically creates/finds the active cart using restaurantId.
   */
  addItem(menuItem: MenuItem, restaurantId: string, restaurantName: string): Observable<any> {
    const current = this.cartItems.value;
    const existing = current.find(i => i.menuItem.id === menuItem.id);

    if (existing) {
      // INCREMENT EXISTING
      const previousQuantity = existing.quantity;
      const newQuantity = previousQuantity + 1;

      this.cartItems.next(current.map(i =>
        i.menuItem.id === menuItem.id ? { ...i, quantity: newQuantity } : i
      ));
      this.persist();

      return this.syncItem(menuItem.id, restaurantId, newQuantity, existing.backendCartId);
    } else {
      // ADD NEW
      const tempItem: CartItem = { menuItem, quantity: 1, restaurantId, restaurantName };
      this.cartItems.next([...current, tempItem]);
      this.persist();

      return this.syncItem(menuItem.id, restaurantId, 1);
    }
  }

  /**
   * Core sync logic: Uses POST /items which handles cart creation on the backend.
   */
  private syncItem(menuItemId: string, restaurantId: string, quantity: number, cartId?: string): Observable<any> {
    const effectiveCartId = cartId || this.cartIdMap.get(restaurantId);

    const payload: any = {
      menuItemId,
      quantity,
      restaurantId: effectiveCartId ? undefined : restaurantId, // Only send restaurantId if we don't have a cartId yet
      cartId: effectiveCartId || '00000000-0000-0000-0000-000000000000' // Guid.Empty if unknown
    };

    return this.api.post<BackendCartItemResponse>(API_ENDPOINTS.CART.ADD_ITEM, payload).pipe(
      tap(resp => {
        // Update local state with real backend IDs
        const items = this.cartItems.value.map(i =>
          i.menuItem.id === menuItemId
            ? { ...i, backendCartId: resp.cartId, backendItemId: resp.id }
            : i
        );
        this.cartItems.next(items);
        this.cartIdMap.set(restaurantId, resp.cartId);
        this.persist();
      }),
      catchError(err => {
        console.error('[CartService] Sync failed:', err);
        // We could add rollback logic here, but for now we let the user retry or refresh
        throw err;
      })
    );
  }

  // ─── Update quantity ──────────────────────────────────────────────────────

  updateQuantity(menuItemId: string, quantity: number): void {
    if (quantity <= 0) { this.removeItem(menuItemId); return; }

    const item = this.cartItems.value.find(i => i.menuItem.id === menuItemId);
    if (!item) return;

    const previousQuantity = item.quantity;
    const cartId = item.backendCartId || this.cartIdMap.get(item.restaurantId);

    // Optimistic update
    this.cartItems.next(this.cartItems.value.map(i =>
      i.menuItem.id === menuItemId ? { ...i, quantity } : i
    ));
    this.persist();

    // Background sync
    if (cartId) {
      this.api.put(API_ENDPOINTS.CART.UPDATE_ITEM, { 
        cartId, 
        menuItemId, 
        quantity 
      })
      .pipe(
        catchError(err => {
          console.error('[CartService] updateQuantity failed:', err);
          this.cartItems.next(this.cartItems.value.map(i =>
            i.menuItem.id === menuItemId ? { ...i, quantity: previousQuantity } : i
          ));
          this.persist();
          return of(null);
        })
      )
      .subscribe();
    } else {
      // Fallback to POST /items which handles cart creation
      this.syncItem(menuItemId, item.restaurantId, quantity).subscribe();
    }
  }

  // ─── Remove item ──────────────────────────────────────────────────────────

  removeItem(menuItemId: string): void {
    const item = this.cartItems.value.find(i => i.menuItem.id === menuItemId);
    if (!item) return;

    const previousItems = this.cartItems.value;
    this.cartItems.next(previousItems.filter(i => i.menuItem.id !== menuItemId));
    this.persist();

    if (item.backendItemId && item.backendCartId) {
      this.api.delete(API_ENDPOINTS.CART.DELETE_ITEM(item.backendItemId, item.backendCartId))
        .pipe(
          catchError(err => {
            console.error('[CartService] removeItem failed:', err);
            this.cartItems.next(previousItems);
            this.persist();
            return of(null);
          })
        )
        .subscribe();
    }
  }

  // ─── Clear ────────────────────────────────────────────────────────────────

  // ─── Proactive Cart Management ──────────────────────────────────────────
 
  /**
   * Ensures a cart exists for this restaurant.
   * If not cached locally, it calls the backend to create/find one.
   */
  ensureCart(restaurantId: string): Observable<string> {
    const cached = this.cartIdMap.get(restaurantId);
    if (cached) return of(cached);

    return this.api.post<BackendCartCreatedResponse>(API_ENDPOINTS.CART.CREATE, { restaurantId }).pipe(
      tap((resp: BackendCartCreatedResponse) => {
        this.cartIdMap.set(restaurantId, resp.cartId);
        this.persist();
      }),
      switchMap((resp: BackendCartCreatedResponse) => of(resp.cartId)),
      catchError(err => {
        console.error('[CartService] ensureCart failed:', err);
        throw err;
      })
    );
  }

  /**
   * Loads existing items for a restaurant from the backend.
   */
  loadCartForRestaurant(restaurantId: string): void {
    this.ensureCart(restaurantId).pipe(
      switchMap((cartId: string) => this.api.get<any[]>(API_ENDPOINTS.CART.GET_ITEMS(cartId))),
      tap((items: any[]) => {
        // Update local items for THIS restaurant only, keeping others
        const otherItems = this.cartItems.value.filter(i => i.restaurantId !== restaurantId);
        const mappedItems: CartItem[] = items.map((i: any) => ({
          menuItem: { id: i.menuItemId, name: i.menuItemName, price: i.unitPrice } as any,
          quantity: i.quantity,
          restaurantId: restaurantId,
          restaurantName: '', // Will be filled by component
          backendItemId: i.id,
          backendCartId: i.cartId
        }));
        this.cartItems.next([...otherItems, ...mappedItems]);
        this.persist();
      })
    ).subscribe();
  }

  clearCart(): void {
    this.cartItems.next([]);
    this.cartIdMap.clear();
    localStorage.removeItem('cart');
    localStorage.removeItem('cart_id_map');
  }
}
