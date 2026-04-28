using OrderService.Application.DTOs.Payment;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private static readonly Random _random = new();

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        public async Task<PaymentResponseDTO> SimulatePaymentAsync(SimulatePaymentDTO dto, string userId)
        {
            var order = await _orderRepository.GetById(dto.OrderId);
            if (order == null)
                throw new NotFoundException("Order", dto.OrderId);

            // Check authorization: user must be the customer or restaurant owner
            await ValidateUserAccess(order, userId);

            var existing = await _paymentRepository.GetByOrderId(dto.OrderId);

            if (existing != null && existing.Status == PaymentStatus.Completed)
                throw new ConflictException("Payment already completed for this order.");

            if (existing == null)
            {
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Method = dto.Method,
                    Amount = order.TotalAmount,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _paymentRepository.AddAsync(payment);
                return MapToDTO(payment);
            }

            return MapToDTO(existing);
        }

        public async Task<PaymentResponseDTO> CompletePaymentAsync(Guid orderId, string userId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null)
                throw new NotFoundException("Order", orderId);

            // Check authorization: user must be the customer or restaurant owner
            await ValidateUserAccess(order, userId);

            var payment = await _paymentRepository.GetByOrderId(orderId);
            if (payment == null)
                throw new NotFoundException("Payment", orderId);

            if (payment.Status == PaymentStatus.Completed)
                throw new ConflictException("Payment already completed.");

            var isSuccess = DeterminePaymentSuccess(payment.Method);

            payment.Status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
            payment.TransactionReference = isSuccess ? GenerateTransactionReference() : null;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);

            return MapToDTO(payment);
        }

        public async Task<PaymentResponseDTO> GetPaymentStatusAsync(Guid orderId, string userId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null)
                throw new NotFoundException("Order", orderId);

            // Check authorization: user must be the customer or restaurant owner
            await ValidateUserAccess(order, userId);

            var payment = await _paymentRepository.GetByOrderId(orderId);
            if (payment == null)
                throw new NotFoundException("Payment", orderId);

            return MapToDTO(payment);
        }

        private static Task ValidateUserAccess(Order order, string userId)
        {
            // Payment is accessible only to the customer who placed the order.
            // Partners view payment status via their sub-order response (SubTotal field).
            if (order.CustomerId != userId)
                throw new ForbiddenException("You do not have access to this payment.");

            return Task.CompletedTask;
        }

        private bool DeterminePaymentSuccess(PaymentMethod method)
        {
            if (method == PaymentMethod.COD)
                return true;

            lock (_random)
            {
                return _random.NextDouble() < 0.8;
            }
        }

        private static string GenerateTransactionReference()
        {
            return $"TXN-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
        }

        private static PaymentResponseDTO MapToDTO(Payment payment)
        {
            return new PaymentResponseDTO
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Method = payment.Method.ToString(),
                Status = payment.Status.ToString(),
                Amount = payment.Amount,
                TransactionReference = payment.TransactionReference,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt
            };
        }
    }
}
