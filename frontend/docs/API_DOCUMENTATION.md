# API Documentation

Base URL: `http://localhost:5000/gateway`

All authenticated endpoints require JWT token in Authorization header:
```
Authorization: Bearer {token}
```

---

## Auth Service (`/gateway/Auth`)

### 1. Register
**POST** `/gateway/Auth/Auth/register`

**Request Body:**
```json
{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "Password123",
  "phoneNumber": "1234567890",
  "role": "Customer"
}
```

**Roles:** `Customer`, `Partner`, `DeliveryAgent`

**Response:** `200 OK`
```json
{
  "message": "Registration successful. Please check your email for OTP."
}
```

**Notes:**
- Partner/DeliveryAgent require admin approval
- OTP sent to email for verification

---

### 2. Login
**POST** `/gateway/Auth/Auth/login`

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "Password123"
}
```

**Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_string"
}
```

**Token Payload:**
```json
{
  "userId": "guid",
  "email": "john@example.com",
  "role": "Customer",
  "emailConfirmed": "true",
  "exp": 1234567890
}
```

---

### 3. Confirm Email
**POST** `/gateway/Auth/Auth/confirm-email`

**Request Body:**
```json
{
  "email": "john@example.com",
  "otp": "123456"
}
```

**Response:** `200 OK`
```json
{
  "message": "Email confirmed successfully"
}
```

---

### 4. Resend Email Confirmation OTP
**POST** `/gateway/Auth/Auth/resend-confirmation-otp`

**Query Params:** `?email=john@example.com`

**Response:** `200 OK`

---

### 5. Forgot Password
**POST** `/gateway/Auth/Auth/forgot-password`

**Query Params:** `?email=john@example.com`

**Response:** `200 OK`
```json
{
  "message": "Password reset OTP sent to email"
}
```

---

### 6. Reset Password
**POST** `/gateway/Auth/Auth/reset-password`

**Request Body:**
```json
{
  "email": "john@example.com",
  "otp": "123456",
  "newPassword": "NewPassword123"
}
```

**Response:** `200 OK`

---

### 7. Change Password
**PUT** `/gateway/Auth/Auth/change-password`

**Auth Required:** Yes

**Request Body:**
```json
{
  "currentPassword": "OldPassword123",
  "newPassword": "NewPassword123"
}
```

**Response:** `200 OK`

---

### 8. Get Profile
**GET** `/gateway/Auth/Auth/profile`

**Auth Required:** Yes

**Response:** `200 OK`
```json
{
  "id": "guid",
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "1234567890",
  "role": "Customer",
  "emailConfirmed": true,
  "isActive": true,
  "twoFactorEnabled": false
}
```

---

### 9. Update Profile
**PUT** `/gateway/Auth/User/profile`

**Auth Required:** Yes

**Request Body:**
```json
{
  "fullName": "John Updated",
  "phoneNumber": "9876543210"
}
```

**Response:** `200 OK`

---

### 10. Get Addresses
**GET** `/gateway/Auth/User/addresses`

**Auth Required:** Yes

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "street": "123 Main St",
    "city": "Mumbai",
    "state": "Maharashtra",
    "pincode": "400001",
    "isDefault": true
  }
]
```

---

### 11. Add Address
**POST** `/gateway/Auth/User/addresses`

**Auth Required:** Yes

**Request Body:**
```json
{
  "street": "123 Main St",
  "city": "Mumbai",
  "state": "Maharashtra",
  "pincode": "400001",
  "isDefault": false
}
```

**Response:** `201 Created`

---

### 12. Update Address
**PUT** `/gateway/Auth/User/addresses/{id}`

**Auth Required:** Yes

**Request Body:**
```json
{
  "street": "456 New St",
  "city": "Mumbai",
  "state": "Maharashtra",
  "pincode": "400002",
  "isDefault": true
}
```

**Response:** `200 OK`

---

### 13. Delete Address
**DELETE** `/gateway/Auth/User/addresses/{id}`

**Auth Required:** Yes

**Response:** `204 No Content`

---

### 14. Request Role Approval (Partner/DeliveryAgent)
**POST** `/gateway/Auth/User/request-role-approval`

**Auth Required:** Yes

**Request Body:**
```json
{
  "requestedRole": "Partner",
  "reason": "I want to add my restaurant"
}
```

**Response:** `200 OK`

---

## Catalog Service (`/gateway/Catalog`)

### 1. Get All Restaurants
**GET** `/gateway/Catalog/Restaurant`

**Query Params (Optional):**
- `searchTerm` - Search by name
- `cuisineId` - Filter by cuisine
- `minRating` - Minimum rating (0-5)
- `isActive` - true/false

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "name": "Pizza Palace",
    "description": "Best pizzas in town",
    "address": "123 Food St, Mumbai",
    "phoneNumber": "1234567890",
    "email": "pizza@example.com",
    "cuisineType": "Italian",
    "rating": 4.5,
    "imageUrl": "https://example.com/image.jpg",
    "openingTime": "10:00:00",
    "closingTime": "22:00:00",
    "isActive": true,
    "isApproved": true,
    "ownerId": "guid"
  }
]
```

---

### 2. Get Restaurant by ID
**GET** `/gateway/Catalog/Restaurant/{id}`

**Response:** `200 OK`
```json
{
  "id": "guid",
  "name": "Pizza Palace",
  "description": "Best pizzas in town",
  "address": "123 Food St, Mumbai",
  "phoneNumber": "1234567890",
  "email": "pizza@example.com",
  "cuisineType": "Italian",
  "rating": 4.5,
  "imageUrl": "https://example.com/image.jpg",
  "openingTime": "10:00:00",
  "closingTime": "22:00:00",
  "isActive": true,
  "isApproved": true,
  "ownerId": "guid"
}
```

---

### 3. Create Restaurant (Partner Only)
**POST** `/gateway/Catalog/Restaurant`

**Auth Required:** Yes (Partner role)

**Request Body:**
```json
{
  "name": "Pizza Palace",
  "description": "Best pizzas in town",
  "address": "123 Food St, Mumbai",
  "phoneNumber": "1234567890",
  "email": "pizza@example.com",
  "cuisineType": "Italian",
  "imageUrl": "https://example.com/image.jpg",
  "openingTime": "10:00:00",
  "closingTime": "22:00:00"
}
```

**Response:** `201 Created`
```json
{
  "id": "guid"
}
```

**Note:** Restaurant requires admin approval before appearing in listings

---

### 4. Update Restaurant (Partner Only)
**PUT** `/gateway/Catalog/Restaurant/{id}`

**Auth Required:** Yes (Partner role, must be owner)

**Request Body:** Same as Create

**Response:** `204 No Content`

---

### 5. Delete Restaurant (Partner/Admin)
**DELETE** `/gateway/Catalog/Restaurant/{id}`

**Auth Required:** Yes

**Response:** `204 No Content`

---

### 6. Get Categories by Restaurant
**GET** `/gateway/Catalog/Category/restaurant/{restaurantId}`

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "name": "Pizzas",
    "description": "Delicious pizzas",
    "restaurantId": "guid",
    "isActive": true
  }
]
```

---

### 7. Create Category (Partner Only)
**POST** `/gateway/Catalog/Category`

**Auth Required:** Yes (Partner role)

**Request Body:**
```json
{
  "name": "Pizzas",
  "description": "Delicious pizzas",
  "restaurantId": "guid"
}
```

**Response:** `201 Created`

---

### 8. Update Category
**PUT** `/gateway/Catalog/Category/{id}`

**Auth Required:** Yes (Partner role)

**Request Body:**
```json
{
  "name": "Updated Pizzas",
  "description": "Updated description",
  "isActive": true
}
```

**Response:** `204 No Content`

---

### 9. Delete Category
**DELETE** `/gateway/Catalog/Category/{id}`

**Auth Required:** Yes (Partner role)

**Response:** `204 No Content`

---

### 10. Get Menu Items by Category
**GET** `/gateway/Catalog/MenuItem/category/{categoryId}`

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "name": "Margherita Pizza",
    "description": "Classic pizza with cheese",
    "price": 299.00,
    "imageUrl": "https://example.com/pizza.jpg",
    "categoryId": "guid",
    "isAvailable": true,
    "isVegetarian": true
  }
]
```

---

### 11. Get Menu Items by Restaurant
**GET** `/gateway/Catalog/MenuItem/restaurant/{restaurantId}`

**Response:** `200 OK` (Same format as above)

---

### 12. Create Menu Item (Partner Only)
**POST** `/gateway/Catalog/MenuItem`

**Auth Required:** Yes (Partner role)

**Request Body:**
```json
{
  "name": "Margherita Pizza",
  "description": "Classic pizza with cheese",
  "price": 299.00,
  "imageUrl": "https://example.com/pizza.jpg",
  "categoryId": "guid",
  "isVegetarian": true
}
```

**Response:** `201 Created`

---

### 13. Update Menu Item
**PUT** `/gateway/Catalog/MenuItem/{id}`

**Auth Required:** Yes (Partner role)

**Request Body:**
```json
{
  "name": "Updated Pizza",
  "description": "Updated description",
  "price": 349.00,
  "imageUrl": "https://example.com/pizza.jpg",
  "isAvailable": true,
  "isVegetarian": true
}
```

**Response:** `204 No Content`

---

### 14. Delete Menu Item
**DELETE** `/gateway/Catalog/MenuItem/{id}`

**Auth Required:** Yes (Partner role)

**Response:** `204 No Content`

---

### 15. Get Service Areas by Restaurant
**GET** `/gateway/Catalog/ServiceArea/restaurant/{restaurantId}`

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "pincode": "400001",
    "areaName": "Colaba",
    "restaurantId": "guid"
  }
]
```

---

### 16. Add Service Area (Partner Only)
**POST** `/gateway/Catalog/ServiceArea`

**Auth Required:** Yes (Partner role)

**Request Body:**
```json
{
  "pincode": "400001",
  "areaName": "Colaba",
  "restaurantId": "guid"
}
```

**Response:** `201 Created`

---

### 17. Delete Service Area
**DELETE** `/gateway/Catalog/ServiceArea/{id}`

**Auth Required:** Yes (Partner role)

**Response:** `204 No Content`

---

### 18. Get All Cuisines
**GET** `/gateway/Catalog/Cuisine`

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "name": "Italian",
    "description": "Italian cuisine"
  }
]
```

---

## Order Service (`/gateway/Order`)

### 1. Get Cart
**GET** `/gateway/Order/carts`

**Auth Required:** Yes (Customer role)

**Response:** `200 OK`
```json
{
  "id": "guid",
  "customerId": "guid",
  "restaurantId": "guid",
  "status": "Active",
  "items": [
    {
      "id": "guid",
      "menuItemId": "guid",
      "menuItemName": "Margherita Pizza",
      "unitPrice": 299.00,
      "quantity": 2,
      "totalPrice": 598.00
    }
  ],
  "totalAmount": 598.00,
  "createdAt": "2026-04-22T10:00:00Z",
  "updatedAt": "2026-04-22T10:05:00Z"
}
```

---

### 2. Add Item to Cart
**POST** `/gateway/Order/carts/items`

**Auth Required:** Yes (Customer role)

**Request Body:**
```json
{
  "menuItemId": "guid",
  "quantity": 2
}
```

**Response:** `200 OK`

**Note:** If cart doesn't exist, it will be created

---

### 3. Update Cart Item Quantity
**PUT** `/gateway/Order/carts/items/{itemId}`

**Auth Required:** Yes (Customer role)

**Request Body:**
```json
{
  "quantity": 3
}
```

**Response:** `200 OK`

---

### 4. Remove Item from Cart
**DELETE** `/gateway/Order/carts/items/{itemId}`

**Auth Required:** Yes (Customer role)

**Response:** `204 No Content`

---

### 5. Clear Cart
**DELETE** `/gateway/Order/carts`

**Auth Required:** Yes (Customer role)

**Response:** `204 No Content`

---

### 6. Place Order
**POST** `/gateway/Order/orders`

**Auth Required:** Yes (Customer role, Email must be confirmed)

**Request Body:**
```json
{
  "cartId": "guid",
  "addressId": "guid",
  "paymentMethod": "CreditCard",
  "deliveryInstructions": "Ring the bell twice",
  "scheduledSlot": "2026-04-22T18:00:00Z"
}
```

**Payment Methods:** `CreditCard`, `DebitCard`, `UPI`, `Cash`

**Response:** `201 Created`
```json
{
  "id": "guid",
  "customerId": "guid",
  "restaurantId": "guid",
  "status": "Pending",
  "street": "123 Main St",
  "city": "Mumbai",
  "state": "Maharashtra",
  "pincode": "400001",
  "deliveryInstructions": "Ring the bell twice",
  "scheduledSlot": "2026-04-22T18:00:00Z",
  "totalAmount": 598.00,
  "cancellationReason": null,
  "createdAt": "2026-04-22T10:00:00Z",
  "updatedAt": "2026-04-22T10:00:00Z",
  "items": [
    {
      "id": "guid",
      "menuItemId": "guid",
      "menuItemName": "Margherita Pizza",
      "unitPrice": 299.00,
      "quantity": 2,
      "totalPrice": 598.00
    }
  ]
}
```

**Validations:**
- Cart must exist and belong to customer
- Cart must be Active
- Cart must have items
- Address must exist and belong to customer
- Restaurant must be active and approved
- Restaurant must deliver to address pincode
- Restaurant must be open (operating hours)
- All menu items must be available
- All categories must be active
- Email must be confirmed

---

### 7. Get Order History
**GET** `/gateway/Order/orders`

**Auth Required:** Yes (Customer or Partner role)

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "status": "Delivered",
    "totalAmount": 598.00,
    "itemCount": 2,
    "createdAt": "2026-04-22T10:00:00Z"
  }
]
```

**Note:** 
- Customer sees their orders
- Partner sees orders for their restaurants

---

### 8. Get Order by ID
**GET** `/gateway/Order/orders/{id}`

**Auth Required:** Yes (Customer role)

**Response:** `200 OK` (Same format as Place Order response)

---

### 9. Cancel Order
**PUT** `/gateway/Order/orders/{id}/cancel`

**Auth Required:** Yes (Customer role)

**Request Body:**
```json
{
  "cancellationReason": "Changed my mind"
}
```

**Response:** `200 OK`

**Constraints:**
- Only allowed in "Pending" status
- Must be within 10 minutes of order creation

---

### 10. Update Order Status (Partner/Admin)
**PUT** `/gateway/Order/orders/{id}/status`

**Auth Required:** Yes (Partner or Admin role)

**Request Body:**
```json
{
  "status": "RestaurantAccepted"
}
```

**Valid Status Transitions:**
- Pending → RestaurantAccepted
- Pending → RestaurantRejected
- RestaurantAccepted → Preparing
- Preparing → ReadyForPickup
- ReadyForPickup → PickedUp
- PickedUp → OutForDelivery
- OutForDelivery → Delivered

**Response:** `204 No Content`

---

### 11. Get Payment by Order ID
**GET** `/gateway/Order/payments/order/{orderId}`

**Auth Required:** Yes (Customer or Partner role)

**Response:** `200 OK`
```json
{
  "id": "guid",
  "orderId": "guid",
  "method": "CreditCard",
  "status": "Completed",
  "amount": 598.00,
  "transactionReference": "TXN-ABC123",
  "createdAt": "2026-04-22T10:00:00Z",
  "updatedAt": "2026-04-22T10:01:00Z"
}
```

---

## Admin Service (`/gateway/Admin`)

### 1. Get Dashboard Stats
**GET** `/gateway/Admin/Dashboard`

**Auth Required:** Yes (Admin role)

**Response:** `200 OK`
```json
{
  "totalUsers": 1500,
  "totalRestaurants": 250,
  "totalOrders": 5000,
  "totalRevenue": 500000.00,
  "pendingApprovals": 15,
  "activeOrders": 45
}
```

---

### 2. Get Pending User Approvals
**GET** `/gateway/Admin/UserApproval/pending`

**Auth Required:** Yes (Admin role)

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "userId": "guid",
    "userName": "John Doe",
    "userEmail": "john@example.com",
    "requestedRole": "Partner",
    "reason": "I want to add my restaurant",
    "status": "Pending",
    "requestedAt": "2026-04-22T10:00:00Z"
  }
]
```

---

### 3. Approve/Reject User Role
**PUT** `/gateway/Admin/UserApproval/{requestId}`

**Auth Required:** Yes (Admin role)

**Request Body:**
```json
{
  "isApproved": true,
  "rejectionReason": null
}
```

**Response:** `200 OK`

---

### 4. Get Pending Restaurant Approvals
**GET** `/gateway/Admin/RestaurantApproval/pending`

**Auth Required:** Yes (Admin role)

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "name": "Pizza Palace",
    "ownerName": "John Doe",
    "ownerEmail": "john@example.com",
    "address": "123 Food St",
    "phoneNumber": "1234567890",
    "cuisineType": "Italian",
    "isApproved": false,
    "createdAt": "2026-04-22T10:00:00Z"
  }
]
```

---

### 5. Approve/Reject Restaurant
**PUT** `/gateway/Admin/RestaurantApproval/{restaurantId}`

**Auth Required:** Yes (Admin role)

**Request Body:**
```json
{
  "isApproved": true
}
```

**Response:** `200 OK`

---

### 6. Get All Users
**GET** `/gateway/Admin/UserManagement`

**Auth Required:** Yes (Admin role)

**Query Params (Optional):**
- `role` - Filter by role
- `isActive` - Filter by active status

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "fullName": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "1234567890",
    "role": "Customer",
    "isActive": true,
    "emailConfirmed": true,
    "createdAt": "2026-04-22T10:00:00Z"
  }
]
```

---

### 7. Activate/Deactivate User
**PUT** `/gateway/Admin/UserManagement/{userId}/status`

**Auth Required:** Yes (Admin role)

**Request Body:**
```json
{
  "isActive": false
}
```

**Response:** `200 OK`

---

### 8. Get All Restaurants (Admin)
**GET** `/gateway/Admin/RestaurantManagement`

**Auth Required:** Yes (Admin role)

**Response:** `200 OK` (Same format as Catalog Restaurant list)

---

### 9. Update Restaurant Status (Admin)
**PUT** `/gateway/Admin/RestaurantManagement/{restaurantId}/status`

**Auth Required:** Yes (Admin role)

**Request Body:**
```json
{
  "isActive": false
}
```

**Response:** `200 OK`

---

### 10. Get All Orders (Admin)
**GET** `/gateway/Admin/Order`

**Auth Required:** Yes (Admin role)

**Query Params (Optional):**
- `status` - Filter by order status
- `restaurantId` - Filter by restaurant

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "customerName": "John Doe",
    "restaurantName": "Pizza Palace",
    "status": "Delivered",
    "totalAmount": 598.00,
    "createdAt": "2026-04-22T10:00:00Z"
  }
]
```

---

### 11. Update Order Status (Admin)
**PUT** `/gateway/Admin/Order/{orderId}/status`

**Auth Required:** Yes (Admin role)

**Request Body:**
```json
{
  "newStatus": "Delivered"
}
```

**Response:** `200 OK`

---

### 12. Generate Reports
**GET** `/gateway/Admin/Report`

**Auth Required:** Yes (Admin role)

**Query Params:**
- `reportType` - `Sales`, `Orders`, `Restaurants`, `Users`
- `startDate` - Start date (YYYY-MM-DD)
- `endDate` - End date (YYYY-MM-DD)

**Response:** `200 OK`
```json
{
  "reportType": "Sales",
  "startDate": "2026-04-01",
  "endDate": "2026-04-30",
  "data": {
    "totalSales": 50000.00,
    "totalOrders": 500,
    "averageOrderValue": 100.00
  }
}
```

---

## Error Responses

All endpoints return consistent error format:

**400 Bad Request**
```json
{
  "message": "Validation error description",
  "statusCode": 400
}
```

**401 Unauthorized**
```json
{
  "message": "Unauthorized access",
  "statusCode": 401
}
```

**403 Forbidden**
```json
{
  "message": "You do not have permission to access this resource",
  "statusCode": 403
}
```

**404 Not Found**
```json
{
  "message": "Resource not found",
  "statusCode": 404
}
```

**500 Internal Server Error**
```json
{
  "message": "An error occurred while processing your request",
  "statusCode": 500
}
```

---

## Notes

1. All datetime fields are in ISO 8601 format (UTC)
2. All monetary values are in decimal format (2 decimal places)
3. GUIDs are in standard format: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
4. Enum values are case-sensitive strings
5. Pagination will be added in future versions
6. Rate limiting may be applied in production
