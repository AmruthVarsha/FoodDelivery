import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { 
  OrderResponseDTO, 
  OrderSummaryDTO, 
  PlaceOrderDTO, 
  CancelOrderDTO, 
  UpdateOrderStatusDTO 
} from '../../shared/models/order.model';

/**
 * Order Service
 * 
 * Manages order operations using backend APIs.
 */
@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private api: ApiService) {}

  /**
   * Place a new order
   * POST /gateway/order/orders
   */
  placeOrder(dto: PlaceOrderDTO): Observable<OrderResponseDTO> {
    return this.api.post<OrderResponseDTO>('/gateway/order/orders', dto);
  }

  /**
   * Get order by ID
   * GET /gateway/order/orders/{id}
   */
  getOrderById(id: string): Observable<OrderResponseDTO> {
    return this.api.get<OrderResponseDTO>(`/gateway/order/orders/${id}`);
  }

  /**
   * Get order history for current user
   * GET /gateway/order/orders/history
   */
  getOrderHistory(): Observable<OrderSummaryDTO[]> {
    return this.api.get<OrderSummaryDTO[]>('/gateway/order/orders/history');
  }

  /**
   * Get my orders (active orders)
   * GET /gateway/order/orders/my-orders
   */
  getMyOrders(): Observable<OrderResponseDTO[]> {
    return this.api.get<OrderResponseDTO[]>('/gateway/order/orders/my-orders');
  }

  /**
   * Cancel order
   * POST /gateway/order/orders/{id}/cancel
   */
  cancelOrder(id: string, dto: CancelOrderDTO): Observable<void> {
    return this.api.post<void>(`/gateway/order/orders/${id}/cancel`, dto);
  }

  /**
   * Update order status (Partner/Admin only)
   * PUT /gateway/order/orders/{id}/status
   */
  updateOrderStatus(id: string, dto: UpdateOrderStatusDTO): Observable<void> {
    return this.api.put<void>(`/gateway/order/orders/${id}/status`, dto);
  }
}
