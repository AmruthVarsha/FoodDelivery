# Quick Start Guide

## Prerequisites
- Node.js 18+ and npm
- Angular CLI 17+
- Backend services running (see backend README)

## Setup

### 1. Install Angular CLI
```bash
npm install -g @angular/cli@17
```

### 2. Create Angular Project
```bash
ng new FoodDelivery-Frontend
? Would you like to add Angular routing? Yes
? Which stylesheet format would you like to use? SCSS
? Do you want to enable Server-Side Rendering (SSR)? No
```

### 3. Install Dependencies
```bash
cd FoodDelivery-Frontend
npm install @angular/material @angular/cdk
npm install ngx-toastr
npm install jwt-decode
npm install rxjs
```

### 4. Configure Environment
Create `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/gateway',
  apiEndpoints: {
    auth: '/Auth',
    catalog: '/Catalog',
    order: '/Order',
    admin: '/Admin'
  }
};
```

## Project Structure Setup

### 1. Create Core Module
```bash
ng generate module core
ng generate service core/services/auth
ng generate service core/services/storage
ng generate service core/services/notification
ng generate guard core/guards/auth
ng generate guard core/guards/role
ng generate interceptor core/interceptors/auth
ng generate interceptor core/interceptors/error
```

### 2. Create Shared Module
```bash
ng generate module shared
ng generate component shared/components/header
ng generate component shared/components/footer
ng generate component shared/components/loading-spinner
```

### 3. Create Feature Modules
```bash
# Auth Module
ng generate module features/auth --routing
ng generate component features/auth/components/login
ng generate component features/auth/components/register
ng generate component features/auth/components/email-confirmation

# Customer Module
ng generate module features/customer --routing
ng generate component features/customer/components/home
ng generate component features/customer/components/restaurant-list
ng generate component features/customer/components/restaurant-detail
ng generate component features/customer/components/cart
ng generate component features/customer/components/checkout
ng generate service features/customer/services/restaurant
ng generate service features/customer/services/cart
ng generate service features/customer/services/order

# Partner Module
ng generate module features/partner --routing
ng generate component features/partner/components/dashboard
ng generate component features/partner/components/restaurant-form
ng generate component features/partner/components/menu-management

# Admin Module
ng generate module features/admin --routing
ng generate component features/admin/components/dashboard
ng generate component features/admin/components/user-approval
ng generate component features/admin/components/restaurant-approval
```

## Key Implementation Files

### 1. Auth Service (`core/services/auth.service.ts`)
```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import jwtDecode from 'jwt-decode';

interface LoginResponse {
  token: string;
  refreshToken: string;
}

interface JwtPayload {
  userId: string;
  email: string;
  role: string;
  emailConfirmed: string;
  exp: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}${environment.apiEndpoints.auth}`;
  private currentUserSubject = new BehaviorSubject<JwtPayload | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadUserFromToken();
  }

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/login`, { email, password })
      .pipe(
        tap(response => {
          localStorage.setItem('token', response.token);
          this.loadUserFromToken();
        })
      );
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Auth/register`, data);
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    try {
      const decoded: JwtPayload = jwtDecode(token);
      return decoded.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  getCurrentUser(): JwtPayload | null {
    return this.currentUserSubject.value;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  isEmailConfirmed(): boolean {
    const user = this.getCurrentUser();
    return user?.emailConfirmed === 'true';
  }

  private loadUserFromToken(): void {
    const token = this.getToken();
    if (token) {
      try {
        const decoded: JwtPayload = jwtDecode(token);
        this.currentUserSubject.next(decoded);
      } catch {
        this.currentUserSubject.next(null);
      }
    }
  }
}
```

### 2. Auth Interceptor (`core/interceptors/auth.interceptor.ts`)
```typescript
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();
    
    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(req);
  }
}
```

### 3. Auth Guard (`core/guards/auth.guard.ts`)
```typescript
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(): boolean {
    if (this.authService.isAuthenticated()) {
      return true;
    }
    
    this.router.navigate(['/auth/login']);
    return false;
  }
}
```

### 4. Role Guard (`core/guards/role.guard.ts`)
```typescript
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredRoles = route.data['roles'] as string[];
    const user = this.authService.getCurrentUser();

    if (user && requiredRoles.includes(user.role)) {
      return true;
    }

    this.router.navigate(['/']);
    return false;
  }
}
```

### 5. App Module Configuration (`app.module.ts`)
```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ToastrModule } from 'ngx-toastr';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { ErrorInterceptor } from './core/interceptors/error.interceptor';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    CoreModule,
    SharedModule,
    ToastrModule.forRoot({
      positionClass: 'toast-top-right',
      preventDuplicates: true,
    })
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

## Running the Application

### 1. Start Backend Services
```bash
# In backend folder
docker-compose up -d  # Start SQL Server & RabbitMQ
# Run services via Visual Studio or individually
```

### 2. Verify Backend
- Gateway: http://localhost:5000/swagger
- Auth: http://localhost:5001/swagger
- Catalog: http://localhost:5002/swagger
- Order: http://localhost:5003/swagger
- Admin: http://localhost:5004/swagger

### 3. Start Frontend
```bash
cd FoodDelivery-Frontend
ng serve
```

### 4. Access Application
Open browser: http://localhost:4200

## Development Workflow

### 1. Create a New Component
```bash
ng generate component features/customer/components/my-component
```

### 2. Create a New Service
```bash
ng generate service features/customer/services/my-service
```

### 3. Add Routing
Update `app-routing.module.ts` or feature routing module

### 4. Test API Integration
Use browser DevTools Network tab to inspect API calls

## Common Tasks

### Add Material UI Component
```bash
ng add @angular/material
```

### Generate Model/Interface
```bash
ng generate interface shared/models/restaurant
```

### Run Tests
```bash
ng test
```

### Build for Production
```bash
ng build --configuration production
```

## Debugging Tips

### 1. Check API Calls
- Open DevTools → Network tab
- Filter by XHR
- Check request/response

### 2. Check JWT Token
```typescript
// In browser console
const token = localStorage.getItem('token');
console.log(JSON.parse(atob(token.split('.')[1])));
```

### 3. Check CORS Issues
- Verify Gateway is running
- Check CORS configuration in Gateway
- Ensure frontend URL matches allowed origins

### 4. Check Authentication
```typescript
// In browser console
const authService = ng.probe(document.body).injector.get('AuthService');
console.log(authService.getCurrentUser());
```

## Next Steps

1. Implement login/register pages
2. Create restaurant listing page
3. Implement cart functionality
4. Add order placement flow
5. Create partner dashboard
6. Implement admin panel

## Resources

- [Angular Documentation](https://angular.io/docs)
- [Angular Material](https://material.angular.io/)
- [RxJS Documentation](https://rxjs.dev/)
- Backend API Documentation: `docs/API_DOCUMENTATION.md`
- Architecture Overview: `docs/ARCHITECTURE.md`
