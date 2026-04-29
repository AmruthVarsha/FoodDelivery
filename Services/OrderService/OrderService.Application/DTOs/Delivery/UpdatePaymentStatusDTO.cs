using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs.Delivery
{
    public class UpdatePaymentStatusDTO
    {
        public PaymentStatus Status { get; set; }
    }
}
