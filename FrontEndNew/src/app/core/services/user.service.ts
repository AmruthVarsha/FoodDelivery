import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiService } from './api.service';
import { API_ENDPOINTS } from '../constants/api-endpoints';
import { ProfileDTO, UpdateProfileDTO, AddressResponseDTO, AddressDTO, UpdateAddressDTO } from '../../shared/models/user.model';

/**
 * User Service
 * 
 * Manages user profile and address operations using UserController.
 */
@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private api: ApiService) {}

  /**
   * Get user profile
   * GET /gateway/auth/User
   */
  getProfile(): Observable<ProfileDTO> {
    return this.api.get<ProfileDTO>(API_ENDPOINTS.USER.GET_PROFILE);
  }

  /**
   * Update user profile
   * PUT /gateway/auth/User
   */
  updateProfile(data: UpdateProfileDTO): Observable<string> {
    return this.api.put<string>(API_ENDPOINTS.USER.UPDATE_PROFILE, data);
  }

  /**
   * Deactivate account
   * PUT /gateway/auth/User/Deactivate
   */
  deactivateAccount(): Observable<string> {
    return this.api.put<string>(API_ENDPOINTS.USER.DEACTIVATE_ACCOUNT, {});
  }

  /**
   * Reactivate account
   * PUT /gateway/auth/User/Reactivate
   */
  reactivateAccount(): Observable<string> {
    return this.api.put<string>(API_ENDPOINTS.USER.REACTIVATE_ACCOUNT, {});
  }

  /**
   * Get all user addresses
   * GET /gateway/auth/User/Addresses
   */
  getAddresses(): Observable<AddressResponseDTO[]> {
    return this.api.get<AddressResponseDTO[]>(API_ENDPOINTS.USER.GET_ADDRESSES);
  }

  /**
   * Get address by ID
   * GET /gateway/auth/User/Address/{id}
   */
  getAddressById(id: string): Observable<AddressResponseDTO> {
    return this.api.get<AddressResponseDTO>(API_ENDPOINTS.USER.GET_ADDRESS_BY_ID(id));
  }

  /**
   * Add new address
   * POST /gateway/auth/User/Address
   */
  addAddress(data: AddressDTO): Observable<AddressResponseDTO> {
    return this.api.post<AddressResponseDTO>(API_ENDPOINTS.USER.ADD_ADDRESS, data);
  }

  /**
   * Update address
   * PUT /gateway/auth/User/Address
   */
  updateAddress(data: UpdateAddressDTO): Observable<AddressResponseDTO> {
    return this.api.put<AddressResponseDTO>(API_ENDPOINTS.USER.UPDATE_ADDRESS, data);
  }

  /**
   * Delete address
   * DELETE /gateway/auth/User/Address/{id}
   */
  deleteAddress(id: string): Observable<string> {
    return this.api.delete<string>(API_ENDPOINTS.USER.DELETE_ADDRESS(id));
  }
}
