# TypeScript Models & Interfaces

## Authentication Models

### LoginRequest
```typescript
export interface LoginRequest {
  email: string;
  password: string;
}
```

### RegisterRequest
```typescript
export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
  role: 'Customer' | 'Partner' | 'DeliveryAgent';
}
```

### AuthResponse
```typescript
export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
}
```

### JwtPayload
```typescript
export interface JwtPayload {
  sub: string; 