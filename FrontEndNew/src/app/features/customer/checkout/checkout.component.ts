import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService, CartItem } from '../../../core/services/cart.service';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';

interface DeliveryAddress {
  line1: string;
  line2: string;
  city: string;
  state: string;
  pincode: string;
}

interface PaymentMethod {
  id: string;
  type: 'card' | 'upi' | 'cod';
  name: string;
  details: string;
  icon: string;
}

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, NavbarComponent],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {
  cartItems: CartItem[] = [];
  deliveryAddress: DeliveryAddress = {
    line1: 'Apt 402, Skyline Heights',
    line2: '88 Market Street',
    city: 'Phagwara',
    state: 'Punjab',
    pincode: '144411'
  };

  paymentMethods: PaymentMethod[] = [
    {
      id: '1',
      type: 'card',
      name: 'Credit/Debit Card',
      details: '**** **** **** 4242',
      icon: 'credit_card'
    },
    {
      id: '2',
      type: 'upi',
      name: 'UPI',
      details: 'Pay via UPI',
      icon: 'payments'
    },
    {
      id: '3',
      type: 'cod',
      name: 'Cash on Delivery',
      details: 'Pay when you receive',
      icon: 'local_atm'
    }
  ];

  selectedPaymentMethod: string = '1';
  promoCode: string = '';
  isProcessing: boolean = false;

  // Expose Math to template
  Math = Math;

  constructor(
    private router: Router,
    private cartService: CartService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCartItems();
    
    // Subscribe to cart changes
    this.cartService.cartItems$.subscribe(items => {
      this.cartItems = items;
      this.cdr.detectChanges();
    });
  }

  loadCartItems(): void {
    this.cartItems = this.cartService.getCartItems();
  }

  updateQuantity(item: CartItem, change: number): void {
    const newQuantity = item.quantity + change;
    this.cartService.updateQuantity(item.menuItem.id, newQuantity);
  }

  removeItem(item: CartItem): void {
    this.cartService.removeItem(item.menuItem.id);
  }

  get subtotal(): number {
    return this.cartItems.reduce((sum, item) => sum + (item.menuItem.price * item.quantity), 0);
  }

  get deliveryFee(): number {
    return this.subtotal > 500 ? 0 : 40;
  }

  get taxes(): number {
    return Math.round(this.subtotal * 0.05); // 5% GST
  }

  get total(): number {
    return this.subtotal + this.deliveryFee + this.taxes;
  }

  applyPromoCode(): void {
    // TODO: Implement promo code logic
    console.log('Applying promo code:', this.promoCode);
  }

  editAddress(): void {
    // TODO: Navigate to address edit page
    console.log('Edit address');
  }

  placeOrder(): void {
    if (this.cartItems.length === 0) {
      return;
    }

    this.isProcessing = true;

    // TODO: Call order service to create order
    setTimeout(() => {
      this.isProcessing = false;
      // Clear cart after successful order
      this.cartService.clearCart();
      // Navigate to order tracking
      this.router.navigate(['/customer/order-tracking', '12345']);
    }, 2000);
  }
}
