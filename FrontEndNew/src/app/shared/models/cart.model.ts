/**
 * Cart Models
 * 
 * TypeScript interfaces for cart-related data matching backend DTOs.
 */

import { MenuItem } from '../../core/services/catalog.service';

/**
 * Cart Status Enum (matches backend)
 */
export enum CartStatus {
  Active = 0,
  Completed = 1,
  Abandoned = 2
}

/**
 * Payment Method Enum (matches backend)
 */
export enum PaymentMethod {
  CashOnDelivery = 0,
  Card = 1,
  UPI = 2,
  Wallet = 3
}

/**
 * Cart Response DTO
 */
export interface CartResponseDTO {
  id: string; // Guid
  userId: string;
  restaurantId: string;
  status: CartStatus;
  createdAt: Date;
  updatedAt: Date;
  items?: CartItemResponseDTO[]; // Optional, populated when needed
}

/**
 * Cart Item Response DTO
 */
export interface CartItemResponseDTO {
  id: string; // Guid
  cartId: string;
  menuItemId: string;
  quantity: number;
  price: number; // Price at time of adding
  menuItem?: MenuItem; // Populated by backend
  createdAt: Date;
}

/**
 * Create Cart DTO
 */
export interface CartDTO {
  restaurantId: string;
  status?: CartStatus;
}

/**
 * Add Cart Item DTO
 */
export interface CartItemDTO {
  cartId: string;
  menuItemId: string;
  quantity: number;
}

/**
 * Update Cart Item DTO
 */
export interface UpdateCartItemDTO {
  id: string; // CartItem ID
  cartId: string;
  menuItemId: string;
  quantity: number;
}

/**
 * Place Order DTO
 */
export interface PlaceOrderDTO {
  cartId: string;
  addressId: string;
  deliveryInstructions?: string;
  scheduledSlot?: string;
  paymentMethod: PaymentMethod;
}
