namespace OrderService.Domain.Enums
{
    public enum RestaurantOrderStatus
    {
        Pending,        // Just placed, waiting for restaurant to accept
        Accepted,       // Restaurant accepted the order
        Rejected,       // Restaurant rejected the order
        Preparing,      // Restaurant is preparing the food
        ReadyForPickup, // Food ready, waiting for delivery agent
        PickedUp,       // Delivery agent picked up from this restaurant
        Delivered,      // Delivery agent confirmed delivery to customer
        Cancelled       // Cancelled (by restaurant or customer)
    }
}
