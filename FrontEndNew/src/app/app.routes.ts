import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'customer',
    loadChildren: () => import('./features/customer/customer.routes').then(m => m.CUSTOMER_ROUTES)
    // No auth guard - publicly accessible
  },
  // Uncomment these routes when the respective modules are implemented
  // {
  //   path: 'partner',
  //   loadChildren: () => import('./features/partner/partner.routes').then(m => m.PARTNER_ROUTES),
  // },
  // {
  //   path: 'delivery',
  //   loadChildren: () => import('./features/delivery/delivery.routes').then(m => m.DELIVERY_ROUTES),
  // },
  // {
  //   path: 'admin',
  //   loadChildren: () => import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES),
  // },
  {
    path: '',
    redirectTo: '/customer/dashboard',
    pathMatch: 'full'
  }
];
