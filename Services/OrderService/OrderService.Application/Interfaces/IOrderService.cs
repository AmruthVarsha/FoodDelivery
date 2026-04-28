using OrderService.Application.DTOs.Order;

namespace OrderService.Application.Interfaces
{
    public interface IOrderService
    {
        /// <summary>Checkout: compile all active carts → one parent Order + N sub-orders.</summary>
        Task<OrderResponseDTO> CheckoutAsync(CheckoutDTO dto, string customerId, string customerName, string token);

        Task<IEnumerable<OrderResponseDTO>> GetOrderHistoryAsync(string customerId);
        Task<OrderResponseDTO> GetOrderByIdAsync(Guid id, string customerId);
        Task CancelOrderAsync(Guid id, string customerId, CancelOrderDTO dto);
    }
}

