using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities
{
    /// <summary>
    /// Sub-order for a specific restaurant within a parent Order.
    /// Partners can only see their own RestaurantOrder.
    /// </summary>
    public class RestaurantOrder
    {
        public Guid Id { get; set; }

        // FK to parent Order
        public Guid OrderId { get; set; }

        // Restaurant info snapshot (so we don't need to call CatalogService to display)
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public string RestaurantAddress { get; set; } = string.Empty; // pickup address for agent

        // Sub-total for this restaurant's items
        public decimal SubTotal { get; set; }

        // Per-restaurant status
        public RestaurantOrderStatus Status { get; set; } = RestaurantOrderStatus.Pending;

        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Order? Order { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
