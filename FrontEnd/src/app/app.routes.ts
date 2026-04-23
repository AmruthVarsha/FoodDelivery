import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password.component';
import { TwoFactorComponent } from './features/auth/two-factor/two-factor.component';

export const routes: Routes = [
  // Default route
  {
    path: '',
    redirectTo: '/auth/login',
    pathMatch: 'full'
  },

  // Auth routes
  {
    path: 'auth/login',
    component: LoginComponent
  },
  {
    path: 'auth/register',
    component: RegisterComponent
  },
  {
    path: 'auth/forgot-password',
    component: ForgotPasswordComponent
  },
  {
    path: 'auth/reset-password',
    component: ResetPasswordComponent
  },
  {
    path: 'auth/two-factor',
    component: TwoFactorComponent
  },

  // Wildcard route (404)
  {
    path: '**',
    redirectTo: '/auth/login'
  }
];
