using OrderService.Application.DTOs.Payment;

namespace OrderService.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> SimulatePaymentAsync(SimulatePaymentDTO dto, string userId);
        Task<PaymentResponseDTO> CompletePaymentAsync(Guid orderId, string userId);
        Task<PaymentResponseDTO> GetPaymentStatusAsync(Guid orderId, string userId);
    }
}
