namespace OrderService.Application.DTOs.Order
{
    /// <summary>
    /// Partner-facing response for a single RestaurantOrder (sub-order).
    /// Shows ONLY this restaurant's items — other restaurants' data is hidden.
    /// Includes parent order info (customer name, delivery address) for context.
    /// </summary>
    public class PartnerOrderResponseDTO
    {
        // Sub-order identity
        public Guid SubOrderId { get; set; }
        public Guid ParentOrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Customer info from parent order (needed to contact/identify customer)
        public string CustomerName { get; set; } = string.Empty;
        public string DeliveryStreet { get; set; } = string.Empty;
        public string DeliveryCity { get; set; } = string.Empty;
        public string DeliveryPincode { get; set; } = string.Empty;
        public string? DeliveryInstructions { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;

        // Only this restaurant's items
        public List<OrderItemResponseDTO> Items { get; set; } = new();
    }
}
