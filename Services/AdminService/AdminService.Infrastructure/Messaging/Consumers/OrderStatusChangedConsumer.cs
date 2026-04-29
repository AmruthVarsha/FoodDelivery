using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using Shared.Events;
using AdminService.Domain.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AdminService.Infrastructure.Messaging.Consumers
{
    public class OrderStatusChangedConsumer : IConsumer<OrderStatusChangedEvent>
    {
        private readonly IOrderSummaryRepository _orderSummaryRepository;
        private readonly ILogger<OrderStatusChangedConsumer> _logger;

        public OrderStatusChangedConsumer(
            IOrderSummaryRepository orderSummaryRepository,
            ILogger<OrderStatusChangedConsumer> logger)
        {
            _orderSummaryRepository = orderSummaryRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
        {
            var message = context.Message;

            // Safely parse the status string — log and skip if unrecognised.
            if (!Enum.TryParse<OrderStatus>(message.Status, ignoreCase: true, out var parsedStatus))
            {
                _logger.LogWarning(
                    "OrderStatusChangedConsumer: unknown status '{Status}' for order {OrderId}. Message skipped.",
                    message.Status, message.OrderId);
                return;
            }

            var existing = await _orderSummaryRepository.GetByOrderIdAsync(message.OrderId);

            if (existing is null)
            {
                await _orderSummaryRepository.AddAsync(new OrderSummary
                {
                    Id = Guid.NewGuid(),
                    OrderId = message.OrderId,
                    CustomerId = message.CustomerId,
                    RestaurantName = message.RestaurantName,
                    TotalAmount = message.TotalAmount,
                    Status = parsedStatus,
                    PaymentMethod = message.PaymentMethod,
                    PaymentStatus = message.PaymentStatus,
                    PlacedAt = message.PlacedAt,
                    LastUpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                await _orderSummaryRepository.UpdateStatusAsync(
                    message.OrderId, parsedStatus, DateTime.UtcNow, message.PaymentMethod, message.PaymentStatus);
            }
        }
    }
}
