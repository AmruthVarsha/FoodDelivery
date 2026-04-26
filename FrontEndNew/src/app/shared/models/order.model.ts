/**
 * Order Models
 * 
 * TypeScript interfaces for order-related data matching backend DTOs.
 */

/**
 * Order Status Enum (matches backend)
 */
export enum OrderStatus {
  Pending = 0,
  RestaurantAccepted = 1,
  RestaurantRejected = 2,
  Preparing = 3,
  ReadyForPickup = 4,
  PickedUp = 5,
  OutForDelivery = 6,
  Delivered = 7,
  CancelledByCustomer = 8,
  CancelledByRestaurant = 9
}

/**
 * Payment Status Enum
 */
export enum PaymentStatus {
  Pending = 0,
  Completed = 1,
  Failed = 2,
  Refunded = 3
}

/**
 * Payment Method Enum
 */
export enum PaymentMethod {
  CashOnDelivery = 0,
  Card = 1,
  UPI = 2,
  Wallet = 3
}

/**
 * Order Item Response DTO
 */
export interface OrderItemResponseDTO {
  id: string;
  menuItemId: string;
  menuItemName: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}

/**
 * Order Response DTO
 */
export interface OrderResponseDTO {
  id: string;
  customerId: string;
  restaurantId: string;
  status: string; // OrderStatus as string
  street: string;
  city: string;
  state: string;
  pincode: string;
  deliveryInstructions?: string;
  scheduledSlot?: string;
  totalAmount: number;
  cancellationReason?: string;
  createdAt: Date;
  updatedAt: Date;
  items: OrderItemResponseDTO[];
}

/**
 * Order Summary DTO (for order history list)
 */
export interface OrderSummaryDTO {
  id: string;
  status: string;
  totalAmount: number;
  itemCount: number;
  createdAt: Date;
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

/**
 * Cancel Order DTO
 */
export interface CancelOrderDTO {
  cancellationReason: string;
}

/**
 * Update Order Status DTO
 */
export interface UpdateOrderStatusDTO {
  status: OrderStatus;
}
