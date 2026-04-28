namespace OrderService.Domain.Enums
{
    /// <summary>
    /// High-level lifecycle of the parent (main) order.
    /// Per-restaurant progress is tracked in RestaurantOrderStatus.
    /// </summary>
    public enum OrderStatus
    {
        Placed,             // Order created, payment pending
        Paid,               // Payment confirmed (COD: set on delivery; Online: set immediately)
        InProgress,         // At least one restaurant has accepted
        OutForDelivery,     // Agent picked up from at least one restaurant
        Delivered,          // Agent marked the full order as delivered
        CancelledByCustomer,
        CancelledBySystem   // e.g., no active agent found, payment failed
    }
}
