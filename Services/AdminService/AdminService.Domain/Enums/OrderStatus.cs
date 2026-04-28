namespace AdminService.Domain.Enums
{
    /// <summary>
    /// Mirrors OrderService.Domain.Enums.OrderStatus exactly.
    /// Must stay in sync with any changes to the OrderService enum.
    /// </summary>
    public enum OrderStatus
    {
        Placed,             // Order created, payment pending
        Paid,               // Payment confirmed
        InProgress,         // At least one restaurant has accepted
        OutForDelivery,     // Agent picked up from at least one restaurant
        Delivered,          // Agent marked the full order as delivered
        CancelledByCustomer,
        CancelledBySystem   // e.g., no active agent found, payment failed
    }
}
