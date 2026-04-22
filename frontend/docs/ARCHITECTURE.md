# System Architecture

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Angular Frontend                         │
│                   (Port 4200)                                │
└────────────────────────┬────────────────────────────────────┘
                         │ HTTP/HTTPS
                         │ JWT Token
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                   API Gateway (Ocelot)                       │
│                     (Port 5000)                              │
│  - Routing                                                   │
│  - CORS                                                      │
│  - Request Aggregation                                       │
└─────┬──────────┬──────────┬──────────┬────────────────────┘
      │          │          │          │
      ▼          ▼          ▼          ▼
┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│   Auth   │ │ Catalog  │ │  Order   │ │  Admin   │
│ Service  │ │ Service  │ │ Service  │ │ Service  │
│ (5001)   │ │ (5002)   │ │ (5003)   │ │ (5004)   │
└────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘
     │            │            │            │
     ▼            ▼            ▼            ▼
┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│ AuthDb   │ │CatalogDb │ │ OrderDb  │ │ AdminDb  │
└──────────┘ └──────────┘ └──────────┘ └──────────┘
     │            │            │            │
     └────────────┴────────────┴────────────┘
                  │
                  ▼
          ┌──────────────┐
          │  RabbitMQ    │
          │  (5672)      │
          └──────────────┘
```

## Service Responsibilities

### 1. Auth Service
**Purpose:** User authentication, authorization, and profile management

**Responsibilities:**
- User registration and login
- JWT token generation and validation
- Email confirmation with OTP
- Password reset
- Two-factor authentication
- Profile management
- Address management
- Role approval requests (Partner/DeliveryAgent)

**Database:** AuthDb
- Users
- Addresses
- OTPTokens
- RefreshTokens
- RoleApprovalRequests

**Events Published:**
- `UserDataSyncEvent` - When user registers
- `UserRoleApprovalRequestEvent` - When user requests role approval

**Events Consumed:**
- `UserRoleApprovalResponseEvent` - When admin approves/rejects role
- `UserUpdateEvent` - When user data needs to be updated

---

### 2. Catalog Service
**Purpose:** Restaurant and menu management

**Responsibilities:**
- Restaurant CRUD operations
- Menu category management
- Menu item management
- Cuisine type management
- Service area management
- Restaurant search and filtering

**Database:** CatalogDb
- Restaurants
- Categories
- MenuItems
- Cuisines
- ServiceAreas

**Events Published:**
- None currently

**Events Consumed:**
- None currently

---

### 3. Order Service
**Purpose:** Order processing and cart management

**Responsibilities:**
- Shopping cart management
- Order placement with validations
- Order status tracking
- Order cancellation
- Payment processing
- Delivery assignment
- Order history

**Database:** OrderDb
- Carts
- CartItems
- Orders
- OrderItems
- Payments
- DeliveryAssignments

**Events Published:**
- `OrderStatusChangedEvent` - When order status changes

**Events Consumed:**
- `AdminOrderStatusUpdateEvent` - When admin updates order status

**External Service Calls:**
- Auth Service: Get address details, validate user
- Catalog Service: Get restaurant details, validate menu items

---

### 4. Admin Service
**Purpose:** Administrative operations and reporting

**Responsibilities:**
- Dashboard statistics
- User approval (Partner/DeliveryAgent)
- Restaurant approval
- User management (activate/deactivate)
- Restaurant management
- Order management
- Report generation

**Database:** AdminDb
- Users (synced from Auth)
- Restaurants (synced from Catalog)
- Orders (synced from Order)
- Reports

**Events Published:**
- `UserRoleApprovalResponseEvent` - When admin approves/rejects role
- `AdminOrderStatusUpdateEvent` - When admin updates order status

**Events Consumed:**
- `UserDataSyncEvent` - Sync user data from Auth
- `OrderStatusChangedEvent` - Sync order status from Order

---

## Data Flow Diagrams

### User Registration Flow (Partner)

```
┌─────────┐     ┌─────────┐     ┌──────────┐     ┌─────────┐
│Frontend │     │ Gateway │     │   Auth   │     │RabbitMQ │
│         │     │         │     │ Service  │     │         │
└────┬────┘     └────┬────┘     └────┬─────┘     └────┬────┘
     │               │               │                │
     │ POST /register│               │                │
     ├──────────────>│               │                │
     │               │ Forward       │                │
     │               ├──────────────>│                │
     │               │               │                │
     │               │               │ Save User      │
     │               │               │ (Pending)      │
     │               │               │                │
     │               │               │ Publish        │
     │               │               │ UserDataSync   │
     │               │               ├───────────────>│
     │               │               │                │
     │               │ 200 OK        │                │
     │               │<──────────────┤                │
     │ 200 OK        │               │                │
     │<──────────────┤               │                │
     │               │               │                │
                                     │                │
                            ┌────────┴────────┐       │
                            │  Admin Service  │       │
                            │                 │       │
                            └────────┬────────┘       │
                                     │                │
                                     │ Consume        │
                                     │ UserDataSync   │
                                     │<───────────────┤
                                     │                │
                                     │ Save User      │
                                     │ to AdminDb     │
```

### Order Placement Flow

```
┌─────────┐  ┌─────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐
│Frontend │  │ Gateway │  │  Order   │  │ Catalog  │  │   Auth   │
│         │  │         │  │ Service  │  │ Service  │  │ Service  │
└────┬────┘  └────┬────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘
     │            │            │             │             │
     │ POST /orders            │             │             │
     ├───────────>│            │             │             │
     │            │ Forward    │             │             │
     │            ├───────────>│             │             │
     │            │            │             │             │
     │            │            │ Get Cart   │             │
     │            │            │ (from DB)  │             │
     │            │            │             │             │
     │            │            │ Get Address│             │
     │            │            ├────────────┼────────────>│
     │            │            │<───────────┼─────────────┤
     │            │            │             │             │
     │            │            │ Get Restaurant           │
     │            │            ├────────────>│             │
     │            │            │<────────────┤             │
     │            │            │             │             │
     │            │            │ Validate:  │             │
     │            │            │ - Service Area           │
     │            │            │ - Operating Hours        │
     │            │            │ - Menu Items             │
     │            │            │             │             │
     │            │            │ Create Order│             │
     │            │            │ Create Payment           │
     │            │            │ Update Cart │             │
     │            │            │             │             │
     │            │            │ Publish     │             │
     │            │            │ OrderStatus │             │
     │            │            │ (RabbitMQ)  │             │
     │            │            │             │             │
     │            │ 201 Created│             │             │
     │            │<───────────┤             │             │
     │ 201 Created│            │             │             │
     │<───────────┤            │             │             │
```

### Admin Approval Flow

```
┌─────────┐  ┌─────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐
│Frontend │  │ Gateway │  │  Admin   │  │RabbitMQ  │  │   Auth   │
│ (Admin) │  │         │  │ Service  │  │          │  │ Service  │
└────┬────┘  └────┬────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘
     │            │            │             │             │
     │ GET /pending-approvals  │             │             │
     ├───────────>│            │             │             │
     │            │ Forward    │             │             │
     │            ├───────────>│             │             │
     │            │            │             │             │
     │            │ 200 OK     │             │             │
     │            │<───────────┤             │             │
     │ 200 OK     │            │             │             │
     │<───────────┤            │             │             │
     │            │            │             │             │
     │ PUT /approve/{id}       │             │             │
     ├───────────>│            │             │             │
     │            │ Forward    │             │             │
     │            ├───────────>│             │             │
     │            │            │             │             │
     │            │            │ Update     │             │
     │            │            │ Approval   │             │
     │            │            │             │             │
     │            │            │ Publish    │             │
     │            │            │ Approval   │             │
     │            │            │ Response   │             │
     │            │            ├────────────>│             │
     │            │            │             │             │
     │            │            │             │ Consume    │
     │            │            │             │ Approval   │
     │            │            │             ├────────────>│
     │            │            │             │             │
     │            │            │             │ Activate   │
     │            │            │             │ User       │
     │            │            │             │ Account    │
     │            │            │             │             │
     │            │ 200 OK     │             │             │
     │            │<───────────┤             │             │
     │ 200 OK     │            │             │             │
     │<───────────┤            │             │             │
```

## Security Architecture

### Authentication Flow

```
1. User Login
   ↓
2. Auth Service validates credentials
   ↓
3. Generate JWT Token
   - userId
   - email
   - role
   - emailConfirmed
   - expiration (24 hours)
   ↓
4. Return token to frontend
   ↓
5. Frontend stores in localStorage
   ↓
6. Frontend sends token in Authorization header
   ↓
7. Gateway forwards to services
   ↓
8. Services validate token
```

### Authorization Levels

**Public Endpoints:**
- GET /Catalog/Restaurant
- GET /Catalog/MenuItem
- GET /Catalog/Cuisine
- POST /Auth/register
- POST /Auth/login

**Customer Only:**
- POST /Order/carts
- POST /Order/orders
- GET /Order/orders (own orders)

**Partner Only:**
- POST /Catalog/Restaurant
- PUT /Catalog/Restaurant
- POST /Catalog/Category
- POST /Catalog/MenuItem

**Admin Only:**
- All /Admin/* endpoints
- User management
- Restaurant approval

**Partner or Admin:**
- PUT /Order/orders/{id}/status

## Database Schema Overview

### AuthDb
```
Users
├── Id (PK)
├── FullName
├── Email (Unique)
├── PasswordHash
├── PhoneNumber
├── Role
├── EmailConfirmed
├── IsActive
└── TwoFactorEnabled

Addresses
├── Id (PK)
├── UserId (FK)
├── Street
├── City
├── State
├── Pincode
└── IsDefault

RoleApprovalRequests
├── Id (PK)
├── UserId (FK)
├── RequestedRole
├── Reason
├── Status
└── RequestedAt
```

### CatalogDb
```
Restaurants
├── Id (PK)
├── Name
├── Description
├── Address
├── PhoneNumber
├── Email
├── CuisineType
├── Rating
├── ImageUrl
├── OpeningTime
├── ClosingTime
├── IsActive
├── IsApproved
└── OwnerId

Categories
├── Id (PK)
├── Name
├── Description
├── RestaurantId (FK)
└── IsActive

MenuItems
├── Id (PK)
├── Name
├── Description
├── Price
├── ImageUrl
├── CategoryId (FK)
├── IsAvailable
└── IsVegetarian

ServiceAreas
├── Id (PK)
├── Pincode
├── AreaName
└── RestaurantId (FK)
```

### OrderDb
```
Carts
├── Id (PK)
├── CustomerId
├── RestaurantId
├── Status
└── Timestamps

Orders
├── Id (PK)
├── CustomerId
├── RestaurantId
├── Status
├── Address fields
├── TotalAmount
├── CancellationReason
└── Timestamps

Payments
├── Id (PK)
├── OrderId (FK)
├── Method
├── Status
├── Amount
├── TransactionReference
└── Timestamps
```

## Event-Driven Communication

### RabbitMQ Events

**UserDataSyncEvent**
```json
{
  "userId": "guid",
  "fullName": "string",
  "email": "string",
  "phoneNumber": "string",
  "role": "string",
  "isActive": true
}
```

**OrderStatusChangedEvent**
```json
{
  "orderId": "guid",
  "customerId": "guid",
  "restaurantName": "string",
  "totalAmount": 0.00,
  "status": "string",
  "timestamp": "datetime"
}
```

**UserRoleApprovalRequestEvent**
```json
{
  "requestId": "guid",
  "userId": "guid",
  "requestedRole": "string",
  "reason": "string"
}
```

**UserRoleApprovalResponseEvent**
```json
{
  "requestId": "guid",
  "userId": "guid",
  "isApproved": true,
  "rejectionReason": "string"
}
```

## Deployment Architecture

### Development
```
Local Machine
├── SQL Server (Docker)
├── RabbitMQ (Docker)
├── Auth Service (IIS Express)
├── Catalog Service (IIS Express)
├── Order Service (IIS Express)
├── Admin Service (IIS Express)
├── Gateway (IIS Express)
└── Frontend (ng serve)
```

### Production (Recommended)
```
Cloud Infrastructure
├── Load Balancer
│   └── Gateway (Multiple instances)
├── Kubernetes Cluster
│   ├── Auth Service (Pods)
│   ├── Catalog Service (Pods)
│   ├── Order Service (Pods)
│   └── Admin Service (Pods)
├── Azure SQL / AWS RDS
│   ├── AuthDb
│   ├── CatalogDb
│   ├── OrderDb
│   └── AdminDb
├── RabbitMQ (Managed Service)
└── CDN
    └── Angular Frontend (Static files)
```

## Performance Considerations

### Caching Strategy
- Restaurant list (5 minutes)
- Menu items (5 minutes)
- User profile (session)
- Service areas (10 minutes)

### Database Indexing
- User.Email (Unique)
- Restaurant.OwnerId
- Order.CustomerId
- Order.RestaurantId
- CartItem.MenuItemId

### API Response Times (Target)
- Authentication: < 200ms
- Restaurant list: < 300ms
- Order placement: < 500ms
- Dashboard stats: < 400ms

## Monitoring & Logging

### Logging (Serilog)
- File logging per service
- Structured logging
- Log levels: Information, Warning, Error

### Metrics to Monitor
- Request count per endpoint
- Response times
- Error rates
- Database query performance
- RabbitMQ queue lengths
- Active user sessions

## Future Enhancements

1. **Real-time Features**
   - SignalR for live order tracking
   - WebSocket for delivery updates

2. **Caching Layer**
   - Redis for session management
   - Cache frequently accessed data

3. **Search Optimization**
   - Elasticsearch for restaurant search
   - Full-text search capabilities

4. **Payment Gateway**
   - Stripe/Razorpay integration
   - Refund processing

5. **Notifications**
   - Push notifications (Firebase)
   - SMS notifications (Twilio)
   - Email templates

6. **Analytics**
   - User behavior tracking
   - Business intelligence dashboard
   - Predictive analytics
