# Food Delivery Platform - Frontend

## Project Overview
Angular-based frontend for a Food Delivery & Restaurant Aggregator platform with microservices architecture.

## Backend Architecture Context

### Microservices
- **Auth Service** (Port 5001): Authentication, User Management, Role Approvals
- **Catalog Service** (Port 5002): Restaurants, Menu Items, Categories, Service Areas
- **Order Service** (Port 5003): Cart, Orders, Payments, Delivery
- **Admin Service** (Port 5004): Admin Dashboard, Approvals, Reports
- **Gateway** (Port 5000): API Gateway using Ocelot

### Technology Stack (Backend)
- ASP.NET Core 10.0
- SQL Server (Separate DB per service)
- RabbitMQ (Event-driven communication)
- Entity Framework Core
- JWT Authentication
- Ocelot API Gateway

## API Gateway Configuration

### Base URL
```
Development: http://localhost:5000/gateway
Production: https://api.fooddelivery.com/gateway
```

### Gateway Routes
- `/gateway/Auth/*` → Auth Service
- `/gateway/Catalog/*` → Catalog Service  
- `/gateway/Order/*` → Order Service
- `/gateway/Admin/*` → Admin Service

### CORS Configuration
Gateway is configured to allow:
- Origins: `http://localhost:4200`, `http://localhost:3000`
- Methods: All
- Headers: All
- Credentials: Enabled (for JWT)

## User Roles & Permissions

### 1. Customer
- Browse restaurants and menus
- Add items to cart
- Place orders
- Track order status
- Manage profile and addresses
- View order history

### 2. Partner (Restaurant Owner)
- Register restaurant (requires admin approval)
- Manage restaurant details
- Manage menu items and categories
- Manage service areas
- View and update order status
- View analytics

### 3. Admin
- Approve/reject partner registrations
- Approve/reject restaurant registrations
- Manage users (activate/deactivate)
- Manage restaurants
- View all orders
- Generate reports
- Dashboard with statistics

### 4. DeliveryAgent
- View available delivery assignments
- Accept delivery tasks
- Update delivery status
- View delivery history

## Authentication Flow

### Registration
1. User registers with email, password, role
2. If role is Partner/DeliveryAgent → Pending approval
3. Email confirmation OTP sent
4. User confirms email with OTP
5. Admin approves role (for Partner/DeliveryAgent)
6. Account activated

### Login
1. User logs in with email/password
2. Backend returns JWT token
3. Token contains: userId, email, role, emailConfirmed
4. Frontend stores token in localStorage
5. Token sent in Authorization header for all requests

### Email Verification
- Required before placing orders
- OTP sent to email
- 6-digit code
- Expires after certain time

### Two-Factor Authentication (Optional)
- Can be enabled in profile
- OTP sent on each login

## Key Business Rules

### Restaurant Management
- Restaurant must be approved by admin before appearing in listings
- Restaurant must be active (IsActive = true)
- Operating hours validation (OpeningTime - ClosingTime)
- Service area validation (delivery pincode must be in service areas)

### Order Placement
1. Cart validation (items available, restaurant active)
2. Address validation (must belong to user)
3. Service area check (restaurant delivers to address pincode)
4. Operating hours check (restaurant must be open)
5. Menu item availability check
6. Category active check
7. Email must be confirmed
8. Payment processing
9. Cart marked as "Ordered"

### Order Status Flow
```
Pending → RestaurantAccepted → Preparing → ReadyForPickup → 
PickedUp → OutForDelivery → Delivered

Alternative flows:
Pending → RestaurantRejected
Pending → CancelledByCustomer (within 10 minutes)
```

### Order Cancellation
- Only allowed in "Pending" status
- Must be within 10 minutes of order creation
- Requires cancellation reason

## Data Sync & Events

### RabbitMQ Events
Backend services communicate via RabbitMQ events:

1. **User Registration** → AdminService gets user data
2. **Role Approval** → AuthService activates account
3. **Order Status Change** → AdminService syncs order status
4. **Admin Order Update** → OrderService syncs back

### Real-time Updates (Future)
- Order status updates (consider SignalR/WebSockets)
- Delivery tracking
- Restaurant availability changes

## Important Notes for Frontend Development

### 1. Always Use Gateway
```typescript
// ✅ CORRECT
const API_URL = 'http://localhost:5000/gateway';

// ❌ WRONG - Never call services directly
const AUTH_URL = 'http://localhost:5001/api';
```

### 2. JWT Token Handling
```typescript
// Token structure
{
  "userId": "guid",
  "email": "user@example.com",
  "role": "Customer|Partner|Admin|DeliveryAgent",
  "emailConfirmed": "true|false",
  "exp": timestamp
}
```

### 3. Error Handling
Backend returns consistent error format:
```json
{
  "message": "Error description",
  "statusCode": 400
}
```

### 4. Validation Rules
- Email: Valid email format
- Password: Minimum 6 characters (backend validation)
- Phone: 10 digits
- Pincode: 6 digits
- Required fields marked in API docs

### 5. Image Handling
- Restaurant images: URL strings
- Menu item images: URL strings
- Store images in cloud storage (Azure Blob/AWS S3)
- Backend stores only URLs

### 6. Pagination
Most list endpoints support pagination:
```
?pageNumber=1&pageSize=10
```

### 7. Search & Filtering
- Restaurant search by name, cuisine
- Filter by rating, distance, cuisine type
- Sort by popularity, rating, delivery time

## Development Workflow

### 1. Start Backend Services
```bash
# In backend folder
docker-compose up -d  # Start SQL Server & RabbitMQ
# Run each service individually or via Visual Studio
```

### 2. Verify Services
- Gateway: http://localhost:5000/swagger
- Auth: http://localhost:5001/swagger
- Catalog: http://localhost:5002/swagger
- Order: http://localhost:5003/swagger
- Admin: http://localhost:5004/swagger

### 3. Start Frontend
```bash
cd frontend
npm install
ng serve
# App runs on http://localhost:4200
```

## Testing Credentials

### Admin User
```
Email: admin@fooddelivery.com
Password: Admin@123
Role: Admin
```
(Create this manually in database or via seed data)

### Test Customer
Register via UI with role "Customer"

### Test Partner
Register via UI with role "Partner" → Requires admin approval

## Common Issues & Solutions

### 1. CORS Error
- Ensure Gateway is running
- Check CORS configuration in Gateway/Program.cs
- Verify frontend URL matches allowed origins

### 2. 401 Unauthorized
- Check if token is expired
- Verify token is sent in Authorization header
- Format: `Bearer {token}`

### 3. 403 Forbidden
- User doesn't have required role
- Email not confirmed (for order placement)
- Account not activated (for Partner/DeliveryAgent)

### 4. Service Unavailable
- Check if backend services are running
- Verify database connection
- Check RabbitMQ is running

## API Documentation
See `docs/API_DOCUMENTATION.md` for detailed API endpoints, request/response formats, and examples.

## Architecture Diagrams
See `docs/ARCHITECTURE.md` for system architecture and data flow diagrams.

## State Management
Consider using:
- NgRx (for complex state)
- Services with BehaviorSubject (for simple state)
- Local storage (for cart, auth token)

## UI/UX Guidelines
- Mobile-first responsive design
- Loading states for all async operations
- Error messages for failed operations
- Success notifications for completed actions
- Confirmation dialogs for destructive actions
- Optimistic UI updates where appropriate

## Performance Considerations
- Lazy load feature modules
- Image optimization and lazy loading
- Debounce search inputs
- Cache restaurant/menu data
- Pagination for large lists
- Virtual scrolling for long lists

## Security Best Practices
- Never store sensitive data in localStorage (only JWT token)
- Validate all user inputs
- Sanitize HTML content
- Use Angular's built-in XSS protection
- Implement route guards for protected routes
- Auto-logout on token expiration
