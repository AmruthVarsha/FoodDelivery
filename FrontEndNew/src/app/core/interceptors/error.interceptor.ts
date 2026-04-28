import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

let isRefreshing = false;
let refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

/**
 * Error Interceptor
 * 
 * Handles HTTP errors globally.
 * - 401 Unauthorized: Attempt to refresh token, if fail logout and redirect to login
 * - 403 Forbidden: Show access denied message
 * - Other errors: Pass through for component handling
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      
      if (error.status === 401) {
        // If the request is for login or refresh itself, don't try to refresh again
        if (req.url.includes('/Auth/Login') || req.url.includes('/Auth/Refresh')) {
          if (!req.url.includes('/Auth/Login')) { // Don't clear on login failure
             authService.clearAuthData();
             router.navigate(['/auth/login']);
          }
          return throwError(() => error);
        }

        if (!isRefreshing) {
          isRefreshing = true;
          refreshTokenSubject.next(null);

          return authService.refreshToken().pipe(
            switchMap((response) => {
              isRefreshing = false;
              // If we get a new token, notify the subject
              if (response && response.token) {
                refreshTokenSubject.next(response.token);
                // Retry the original request with the new token
                return next(req.clone({
                  setHeaders: {
                    Authorization: `Bearer ${response.token}`
                  }
                }));
              }
              
              // If no token in response, something is wrong
              authService.clearAuthData();
              router.navigate(['/auth/login']);
              return throwError(() => new Error('Refresh failed'));
            }),
            catchError((refreshError) => {
              isRefreshing = false;
              authService.clearAuthData();
              router.navigate(['/auth/login']);
              return throwError(() => refreshError);
            })
          );
        } else {
          // If we are already refreshing, wait for the new token
          return refreshTokenSubject.pipe(
            filter(token => token !== null),
            take(1),
            switchMap(token => {
              return next(req.clone({
                setHeaders: {
                  Authorization: `Bearer ${token}`
                }
              }));
            })
          );
        }
      }

      if (error.status === 403) {
        // Forbidden - User doesn't have permission
        console.warn('Access denied. Insufficient permissions.');
      }

      // Pass the error to the component for specific handling
      return throwError(() => error);
    })
  );
};
