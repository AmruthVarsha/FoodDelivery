/**
 * User Models
 * 
 * TypeScript interfaces for user-related data.
 */

import { RoleEnum } from './auth.model';

/**
 * User Profile DTO
 * Matches backend AuthService.Application.DTOs.ProfileDTO
 */
export interface ProfileDTO {
  userId: string;
  fullName: string;
  email: string;
  phoneNo: string;
  role: string; // Backend returns role as string
  isEmailConfirmed: boolean;
  isTwoFactorEnabled: boolean; // Now always returned from backend
  isActive: boolean;           // Whether the account is active (can be toggled by Admin)
  createdAt?: Date;             // Optional — not yet tracked in User entity
  accountStatus?: AccountStatus; // Computed locally from isActive for display
  addresses?: AddressResponseDTO[];
}

/**
 * Account Status Enum
 */
export enum AccountStatus {
  Active = 0,
  Inactive = 1,
  Suspended = 2,
  PendingApproval = 3
}

/**
 * Address Response DTO
 */
export interface AddressResponseDTO {
  id: string; // Guid
  userId: string;
  street: string;
  city: string;
  state: string;
  pincode: string;
}

/**
 * Update Profile DTO
 * Matches backend update request - all fields required
 */
export interface UpdateProfileDTO {
  fullName: string;
  email: string;
  phoneNo: string;
}

/**
 * Address DTO
 */
export interface AddressDTO {
  street: string;
  city: string;
  state: string;
  pincode: string;
}

/**
 * Update Address DTO
 */
export interface UpdateAddressDTO {
  id: string; // Guid
  street: string;
  city: string;
  state: string;
  pincode: string;
}
