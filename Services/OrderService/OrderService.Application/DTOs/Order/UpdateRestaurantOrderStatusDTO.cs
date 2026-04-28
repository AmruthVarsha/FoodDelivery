using System.ComponentModel.DataAnnotations;
using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs.Order
{
    /// <summary>
    /// Partner-only: updates the status of their RestaurantOrder (sub-order).
    /// Allowed transitions: Pending → Accepted → Preparing → ReadyForPickup.
    /// Partners can also set Rejected or Cancelled.
    /// </summary>
    public class UpdateRestaurantOrderStatusDTO
    {
        [Required]
        public RestaurantOrderStatus Status { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }
    }
}
