using OrderService.Application.DTOs.Order;

namespace OrderService.Application.Interfaces
{
    public interface IRestaurantOrderService
    {
        /// <summary>Partner: get all sub-orders for their restaurant.</summary>
        Task<IEnumerable<PartnerOrderResponseDTO>> GetOrdersForRestaurantAsync(Guid restaurantId);

        /// <summary>Partner: get a specific sub-order (must belong to their restaurant).</summary>
        Task<PartnerOrderResponseDTO> GetSubOrderByIdAsync(Guid subOrderId, Guid restaurantId);

        /// <summary>
        /// Partner: update sub-order status.
        /// Allowed: Pending→Accepted, Accepted→Preparing, Preparing→ReadyForPickup.
        /// Also allowed: Rejected (from Pending), Cancelled (from Accepted/Preparing).
        /// </summary>
        Task<PartnerOrderResponseDTO> UpdateSubOrderStatusAsync(
            Guid subOrderId, Guid restaurantId, UpdateRestaurantOrderStatusDTO dto);
    }
}
