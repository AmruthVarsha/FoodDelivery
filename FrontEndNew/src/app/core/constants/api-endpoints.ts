/**
 * API Endpoints
 * 
 * Centralized constants for all API endpoints.
 * Organized by service (Auth, Catalog, Order, Admin).
 */

export const API_ENDPOINTS = {
  
  // ============================================
  // AUTH SERVICE ENDPOINTS
  // ============================================
  AUTH: {
    REGISTER: '/gateway/auth/Auth/Register',
    LOGIN: '/gateway/auth/Auth/Login',
    LOGOUT: '/gateway/auth/Auth/Logout',
    REFRESH: '/gateway/auth/Auth/Refresh',
    ME: '/gateway/auth/Auth/Me',
    
    // Password Management
    FORGOT_PASSWORD: '/gateway/auth/Auth/ForgotPassword',
    RESET_PASSWORD: '/gateway/auth/Auth/ResetPassword',
    CHANGE_PASSWORD: '/gateway/auth/Auth/ChangePassword',
    
    // Email Confirmation
    SEND_EMAIL_CONFIRMATION_OTP: '/gateway/auth/Auth/SendEmailConfirmationOTP',
    CONFIRM_EMAIL: '/gateway/auth/Auth/ConfirmEmail',
    
    // Two-Factor Authentication
    SET_TWO_FACTOR_AUTH: '/gateway/auth/Auth/SetTwoFactorAuth',
    TWO_FACTOR_AUTH: '/gateway/auth/Auth/TwoFactorAuth',
    VERIFY_OTP: '/gateway/auth/Auth/VerifyOTP',
    
    // Admin Only
    PROMOTE_ROLE: '/gateway/auth/Auth/PromoteRole',
    CHANGE_ACCOUNT_STATUS: '/gateway/auth/Auth/ChangeAccountStatus',
    PENDING_REQUESTS: '/gateway/auth/Auth/PendingRequests',
    APPROVE_REQUEST: (email: string) => `/gateway/auth/Auth/ApproveRequest/${email}`,
  },

  // ============================================
  // USER SERVICE ENDPOINTS
  // ============================================
  USER: {
    // Profile Management
    GET_PROFILE: '/gateway/auth/User',
    UPDATE_PROFILE: '/gateway/auth/User',
    DEACTIVATE_ACCOUNT: '/gateway/auth/User/Deactivate',
    REACTIVATE_ACCOUNT: '/gateway/auth/User/Reactivate',
    
    // Address Management (Customer only)
    GET_ADDRESSES: '/gateway/auth/User/Addresses',
    GET_ADDRESS_BY_ID: (id: string) => `/gateway/auth/User/Address/${id}`,
    ADD_ADDRESS: '/gateway/auth/User/Address',
    UPDATE_ADDRESS: '/gateway/auth/User/Address',
    DELETE_ADDRESS: (id: string) => `/gateway/auth/User/Address/${id}`,
    
    // Admin Only
    GET_ALL_USERS: '/gateway/auth/User/AllUsers',
    GET_USER_BY_ID: (id: string) => `/gateway/auth/User/${id}`,
  },

  // ============================================
  // CATALOG SERVICE ENDPOINTS
  // ============================================
  CATALOG: {
    // Restaurants
    RESTAURANTS: '/gateway/catalog/Restaurant/restaurants',
    RESTAURANT_BY_ID: (id: string) => `/gateway/catalog/Restaurant/restaurant/${id}`,
    RESTAURANT_SEARCH: '/gateway/catalog/Restaurant/search',
    
    // Categories
    CATEGORIES: '/gateway/catalog/Category',
    CATEGORY_BY_ID: (id: string) => `/gateway/catalog/Category/${id}`,
    
    // Menu Items
    MENU_ITEMS: '/gateway/catalog/MenuItem',
    MENU_ITEM_BY_ID: (id: string) => `/gateway/catalog/MenuItem/${id}`,
    MENU_ITEMS_BY_RESTAURANT: (restaurantId: string) => `/gateway/catalog/MenuItem/restaurant/${restaurantId}`,
    MENU_ITEMS_BY_CATEGORY: (categoryId: string) => `/gateway/catalog/MenuItem/category/${categoryId}`,
  },

  // ============================================
  // ORDER SERVICE ENDPOINTS
  // ============================================
  ORDER: {
    // Orders
    ORDERS: '/gateway/order/Order',
    ORDER_BY_ID: (id: number) => `/gateway/order/Order/${id}`,
    CREATE_ORDER: '/gateway/order/Order',
    UPDATE_ORDER_STATUS: (id: number) => `/gateway/order/Order/${id}/status`,
    CANCEL_ORDER: (id: number) => `/gateway/order/Order/${id}/cancel`,
    
    // Customer Orders
    MY_ORDERS: '/gateway/order/Order/my-orders',
    ORDER_HISTORY: '/gateway/order/Order/history',
    
    // Partner Orders
    RESTAURANT_ORDERS: (restaurantId: number) => `/gateway/order/Order/restaurant/${restaurantId}`,
    
    // Payments
    PAYMENTS: '/gateway/order/Payment',
    PAYMENT_BY_ID: (id: number) => `/gateway/order/Payment/${id}`,
    PROCESS_PAYMENT: '/gateway/order/Payment/process',
  },

  // ============================================
  // ADMIN SERVICE ENDPOINTS
  // ============================================
  ADMIN: {
    // Dashboard
    DASHBOARD: '/gateway/admin/Dashboard',
    DASHBOARD_STATS: '/gateway/admin/Dashboard/stats',
    
    // User Management
    USERS: '/gateway/admin/UserManagement',
    USER_BY_ID: (id: string) => `/gateway/admin/UserManagement/${id}`,
    UPDATE_USER_STATUS: '/gateway/admin/UserManagement/status',
    
    // Restaurant Management
    RESTAURANTS: '/gateway/admin/RestaurantManagement',
    RESTAURANT_BY_ID: (id: number) => `/gateway/admin/RestaurantManagement/${id}`,
    APPROVE_RESTAURANT: (id: number) => `/gateway/admin/RestaurantManagement/${id}/approve`,
    REJECT_RESTAURANT: (id: number) => `/gateway/admin/RestaurantManagement/${id}/reject`,
    
    // Restaurant Approval
    PENDING_RESTAURANTS: '/gateway/admin/RestaurantApproval/pending',
    APPROVE_RESTAURANT_REQUEST: (id: number) => `/gateway/admin/RestaurantApproval/${id}/approve`,
    REJECT_RESTAURANT_REQUEST: (id: number) => `/gateway/admin/RestaurantApproval/${id}/reject`,
    
    // User Approval
    PENDING_USERS: '/gateway/admin/UserApproval/pending',
    APPROVE_USER_REQUEST: (id: string) => `/gateway/admin/UserApproval/${id}/approve`,
    REJECT_USER_REQUEST: (id: string) => `/gateway/admin/UserApproval/${id}/reject`,
    
    // Orders
    ALL_ORDERS: '/gateway/admin/Order/all',
    ORDER_DETAILS: (id: number) => `/gateway/admin/Order/${id}`,
    
    // Reports
    REPORTS: '/gateway/admin/Report',
    REVENUE_REPORT: '/gateway/admin/Report/revenue',
    USER_GROWTH_REPORT: '/gateway/admin/Report/user-growth',
    ORDER_STATISTICS: '/gateway/admin/Report/order-statistics',
  },

} as const;
