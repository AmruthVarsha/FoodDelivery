/**
 * Authentication Models
 * 
 * TypeScript interfaces for authentication-related data.
 * These match your backend DTOs.
 */

/**
 * Login Request DTO
 */
export interface LoginDTO {
  email: string;
  password: string;
}

/**
 * Login Response DTO
 */
export interface LoginResponseDTO {
  token: string | null;  // JWT access token (null if 2FA required)
  refreshToken: RefreshTokenDTO | null;  // Refresh token (null if 2FA required)
  requireTwoFactor?: boolean;  // True if 2FA is required (matches backend property name)
  message?: string;
}

/**
 * Refresh Token DTO
 */
export interface RefreshTokenDTO {
  token: string;
  expiresAt: Date;
}

/**
 * Registration Request DTO
 */
export interface RegistrationDTO {
  fullName: string;
  email: string;
  phoneNo: string;
  password: string;
  confirmPassword: string;
  role: RoleEnum;
}

/**
 * User Role Enum (matches backend)
 */
export enum RoleEnum {
  Customer = 0,
  Partner = 1,
  DeliveryAgent = 2,
  Admin = 3
}

/**
 * Confirm Email DTO
 */
export interface ConfirmEmailDTO {
  email: string;
  otp: string;
}

/**
 * Verify Two-Factor OTP DTO
 * Backend expects Token field (not otp)
 */
export interface VerifyTwoFactorDTO {
  email: string;
  token: string;  // Backend expects 'Token' field
}

/**
 * Change Password DTO
 * Matches backend ChangePasswordDTO
 */
export interface ChangePasswordDTO {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

/**
 * Reset Password DTO
 */
export interface ResetPasswordDTO {
  email: string;
  token: string;
  newPassword: string;
  confirmNewPassword: string;
}

/**
 * Decoded JWT Token
 * Contains user information extracted from the JWT
 */
export interface DecodedToken {
  email: string;
  role: string;
  name: string;
  exp: number;  // Expiration timestamp
  iat: number;  // Issued at timestamp
}
