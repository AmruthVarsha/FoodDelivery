import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { MenuItem } from './catalog.service';

export interface CartItem {
  menuItem: MenuItem;
  quantity: number;
  restaurantId: string;
  restaurantName: string;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItems = new BehaviorSubject<CartItem[]>([]);
  public cartItems$ = this.cartItems.asObservable();

  constructor() {
    // Load cart from localStorage on init
    this.loadCartFromStorage();
  }

  private loadCartFromStorage(): void {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      try {
        const items = JSON.parse(savedCart);
        this.cartItems.next(items);
      } catch (error) {
        console.error('Error loading cart from storage:', error);
      }
    }
  }

  private saveCartToStorage(): void {
    localStorage.setItem('cart', JSON.stringify(this.cartItems.value));
  }

  getCartItems(): CartItem[] {
    return this.cartItems.value;
  }

  addItem(menuItem: MenuItem, restaurantId: string, restaurantName: string): void {
    const currentItems = this.cartItems.value;
    const existingItem = currentItems.find(item => item.menuItem.id === menuItem.id);

    if (existingItem) {
      existingItem.quantity++;
    } else {
      currentItems.push({
        menuItem,
        quantity: 1,
        restaurantId,
        restaurantName
      });
    }

    this.cartItems.next([...currentItems]);
    this.saveCartToStorage();
  }

  updateQuantity(menuItemId: string, quantity: number): void {
    const currentItems = this.cartItems.value;
    const item = currentItems.find(item => item.menuItem.id === menuItemId);

    if (item) {
      if (quantity <= 0) {
        this.removeItem(menuItemId);
      } else {
        item.quantity = quantity;
        this.cartItems.next([...currentItems]);
        this.saveCartToStorage();
      }
    }
  }

  removeItem(menuItemId: string): void {
    const currentItems = this.cartItems.value.filter(
      item => item.menuItem.id !== menuItemId
    );
    this.cartItems.next(currentItems);
    this.saveCartToStorage();
  }

  clearCart(): void {
    this.cartItems.next([]);
    localStorage.removeItem('cart');
  }

  getCartCount(): number {
    return this.cartItems.value.reduce((sum, item) => sum + item.quantity, 0);
  }

  getCartTotal(): number {
    return this.cartItems.value.reduce(
      (sum, item) => sum + (item.menuItem.price * item.quantity),
      0
    );
  }
}

