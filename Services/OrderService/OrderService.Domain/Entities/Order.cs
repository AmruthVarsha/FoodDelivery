using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        // Customer info snapshot (from JWT at checkout time)
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        // Delivery address (single drop-off for the entire order)
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string? DeliveryInstructions { get; set; }
        public string? ScheduledSlot { get; set; }

        // Financials & state
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Placed;
        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation — one sub-order per restaurant
        public ICollection<RestaurantOrder> RestaurantOrders { get; set; } = new List<RestaurantOrder>();
        public Payment? Payment { get; set; }
        public DeliveryAssignment? DeliveryAssignment { get; set; }
    }
}