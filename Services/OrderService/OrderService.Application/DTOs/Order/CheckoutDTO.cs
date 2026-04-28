using System.ComponentModel.DataAnnotations;
using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs.Order
{
    /// <summary>
    /// Input DTO for placing an order from ALL active carts.
    /// The system auto-discovers the carts for the current user.
    /// </summary>
    public class CheckoutDTO
    {
        [Required]
        public Guid AddressId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(300)]
        public string? DeliveryInstructions { get; set; }

        [StringLength(100)]
        public string? ScheduledSlot { get; set; }
    }
}
