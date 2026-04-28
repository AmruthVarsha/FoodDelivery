# Cart, Checkout, Order & Payment Redesign - Implementation Plan

## User Decisions & Strategy

### 1. Payment Gateway (COD & Online)
- **Status**: Real Razorpay integration planned for later.
- **Approach**: Implement a `PaymentMethod` abstraction with two modes:
  - `COD` (Cash on Delivery): Immediate success simulation.
  - `Online` (Razorpay-ready): Designed to handle gateway callbacks/responses.
- **Implementation**: Use an `IPaymentGateway` interface to decouple the `PaymentService` from specific providers.

### 2. Delivery Agent Assignment
- **Status**: Auto-assignment based on activity and location (pincode).
- **Alternative (Agent Profiles)**: Instead of modifying the core `User` model, we will create a `DeliveryAgentProfile` entity within the `OrderService`.
- **Fields**: `UserId`, `IsActive`, `CurrentPincode`, `LastSeenAt`.
- **Logic**: When an order is placed, the system finds an active agent in the restaurant's pincode area.

### 3. Multi-Restaurant Cart & Sub-Order Strategy
- **Status**: Cart per restaurant.
- **Approach**: **Backend-driven Carts**. The `Cart` table will support multiple active entries per user (one per `RestaurantId`).
- **Grouped Checkout**: When placing an order, the system will:
  1. Compile all active carts for the user.
  2. Create a single **Main Order** (Parent) for tracking payment and overall delivery.
  3. Create individual **Restaurant Orders** (Sub-Orders) for each restaurant involved.
- **Benefits**: 
  - **Privacy**: Restaurants only see their own `RestaurantOrder` and its items.
  - **Granular Status**: Restaurant A can be "Ready for Pickup" while Restaurant B is still "Preparing".
  - **Delivery Agent View**: Agents see the "Entire Order" (all Sub-Orders) to coordinate pickups.

### 4. Role-Based Order Handling
- **Restaurants (Partners)**: 
  - Access only their `RestaurantOrder`.
  - Update status: `Accepted` → `Preparing` → `ReadyForPickup`.
  - Customer info (Name/ID) is pulled from the Order snapshot (initially from JWT).
- **Delivery Agents**: 
  - Access the `Main Order` and all linked `RestaurantOrders`.
  - View all pickup locations (Restaurant addresses).
  - Update status: `PickedUp` (per sub-order) and `Delivered` (overall).
- **Customers**: 
  - View the unified `Main Order` with status tracking for each sub-restaurant.

---

## Current System Issues

### Cart
- **UX**: Requires manually creating a cart first, then adding items separately.
- **Dependency**: Cart is tied to a restaurant at creation time.
- **Logic**: Multiple active carts per user allowed.
- **States**: `CartStatus.Abandoned` is set when the last item is deleted (unintuitive).
- **Checkout**: Checkout service is redundant and lacks validation.

### Orders & Payment
- **Coupling**: Payment is created inside `PlaceOrderAsync` (mixing concerns).
- **Simulation**: `PaymentService` simulation is scattered and confusing.
- **Dead Code**: Separate `PaymentService` microservice is empty.
- **Process**: No concept of `PaymentMethod` at checkout.
- **Transitions**: Granular and confusing status transitions (e.g., `PickedUp` vs `OutForDelivery`).

---

## Proposed New Flow

`Add to Cart (upsert) → View Checkout Summary → Place Order → Process Payment → Fulfillment`

### Key Principles
1. **Cart per Restaurant** — Users can have multiple carts, one for each restaurant they've added items from.
2. **Backend-First** — Carts are stored in the database to ensure multi-device synchronization.
3. **Agent Profiles** — Delivery-specific data is kept in a separate profile to keep the User model clean.
4. **Decoupled Payment** — Abstracted gateway logic to support COD and future Razorpay.
5. **Simplified Statuses** — Clean transition from Pending to Delivered.

---

## Proposed Changes

### 1. Domain Layer (`OrderService.Domain`)
- **Cart**: Keep `RestaurantId`, simplify status.
- **Order (Parent)**: Contains `CustomerId`, `TotalAmount`, `OverallStatus`, `DeliveryAddress`.
- **RestaurantOrder (Sub-Order)** [NEW]: Contains `RestaurantId`, `SubTotal`, `Status`, `ICollection<OrderItem>`.
- **OrderItem**: Linked to `RestaurantOrder`.
- **Payment**: Add `PaymentMethod` (COD/Online).
- **DeliveryAgentProfile** [NEW]: Linked to `UserId`, contains `IsActive`, `CurrentPincode`.

### 2. Application Layer (`OrderService.Application`)
- **CartService**: Support `UpsertCartItemAsync`, `GetCartByRestaurantAsync`, `GetAllActiveCartsAsync`.
- **OrderManagementService**: 
  - `CreateOrderFromCartsAsync`: Compiles all active carts into one Parent Order + N Sub-Orders.
- **PaymentService**: Support `COD` and `Online` gateways via `IPaymentGateway`.
- **DeliveryService**: Implement `AutoAssignDeliveryAgent` using `DeliveryAgentProfile` lookups.

### 3. API Layer (`OrderService.API`)
- **CartController**: GET/POST/DELETE on `/api/cart`.
- **OrderController**: Clean RESTful endpoints for history and placement.
- **PaymentController**: Explicit endpoints for processing payments.

### 4. Infrastructure
- New events: `OrderConfirmedEvent`, `PaymentProcessedEvent`.

---

## Verification Plan
1. **Automated**: Unit tests for Cart upsert logic and Payment flows.
2. **Manual**: E2E testing of the new "Add to Cart" -> "Payment" -> "Delivery" flow.
