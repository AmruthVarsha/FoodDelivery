import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { API_ENDPOINTS } from '../constants/api-endpoints';
import {
  DeliveryOrderResponseDTO,
  UpdateDeliveryStatusDTO,
  AgentProfileResponseDTO,
  UpsertAgentProfileDTO
} from '../../shared/models/order.model';

@Injectable({
  providedIn: 'root'
})
export class DeliveryService {

  constructor(private api: ApiService) {}

  /**
   * Agent: get currently assigned orders with full pickup/dropoff details.
   * GET /gateway/order/delivery/assignments
   */
  getAssignments(): Observable<DeliveryOrderResponseDTO[]> {
    return this.api.get<DeliveryOrderResponseDTO[]>(API_ENDPOINTS.DELIVERY.ASSIGNMENTS);
  }

  /**
   * Agent: update delivery status.
   * Transitions: Assigned → PickedUp → Delivered.
   * PUT /gateway/order/delivery/assignments/{id}/status
   */
  updateDeliveryStatus(assignmentId: string, dto: UpdateDeliveryStatusDTO): Observable<DeliveryOrderResponseDTO> {
    return this.api.put<DeliveryOrderResponseDTO>(API_ENDPOINTS.DELIVERY.UPDATE_STATUS(assignmentId), dto);
  }

  /**
   * Agent: get their profile (isActive, currentPincode).
   * GET /gateway/order/delivery/profile
   */
  getProfile(): Observable<AgentProfileResponseDTO> {
    return this.api.get<AgentProfileResponseDTO>(API_ENDPOINTS.DELIVERY.PROFILE);
  }

  /**
   * Agent: create or update profile.
   * PUT /gateway/order/delivery/profile
   */
  upsertProfile(dto: UpsertAgentProfileDTO): Observable<AgentProfileResponseDTO> {
    return this.api.put<AgentProfileResponseDTO>(API_ENDPOINTS.DELIVERY.PROFILE, dto);
  }
}
