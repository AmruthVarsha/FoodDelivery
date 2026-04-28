import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { API_ENDPOINTS } from '../constants/api-endpoints';
import {
  OrderResponseDTO,
  CheckoutDTO,
  CancelOrderDTO
} from '../../shared/models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private api: ApiService) {}

  /**
   * Customer: checkout — compiles ALL active carts into one order.
   * POST /gateway/order/orders/checkout
   */
  checkout(dto: CheckoutDTO): Observable<OrderResponseDTO> {
    return this.api.post<OrderResponseDTO>(API_ENDPOINTS.ORDER.CHECKOUT, dto);
  }

  /**
   * Customer: get full order history (grouped by restaurant).
   * GET /gateway/order/orders/my
   */
  getMyOrders(): Observable<OrderResponseDTO[]> {
    return this.api.get<OrderResponseDTO[]>(API_ENDPOINTS.ORDER.MY_ORDERS);
  }

  /**
   * Customer: get a specific order by ID.
   * GET /gateway/order/orders/{id}
   */
  getOrderById(id: string): Observable<OrderResponseDTO> {
    return this.api.get<OrderResponseDTO>(API_ENDPOINTS.ORDER.ORDER_BY_ID(id));
  }

  /**
   * Customer: cancel an order (within 10 min, before any restaurant accepts).
   * PUT /gateway/order/orders/{id}/cancel
   */
  cancelOrder(id: string, dto: CancelOrderDTO): Observable<void> {
    return this.api.put<void>(API_ENDPOINTS.ORDER.CANCEL_ORDER(id), dto);
  }
}
