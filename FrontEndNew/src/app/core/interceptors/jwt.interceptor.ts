import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

/**
 * JWT Interceptor
 * 
 * Automatically adds the JWT token to the Authorization header of every HTTP request.
 * This is a functional interceptor (modern Angular approach).
 * 
 * How it works:
 * 1. Gets the access token from AuthService
 * 2. Clones the request and adds Authorization header
 * 3. Passes the modified request to the next handler
 */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getAccessToken();

  // Always clone with credentials to allow cookies (like refresh token) to be sent
  let modifiedReq = req.clone({
    withCredentials: true
  });

  // If token exists, add Authorization header
  if (token) {
    modifiedReq = modifiedReq.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  // Pass the request to the next handler
  return next(modifiedReq);
};
