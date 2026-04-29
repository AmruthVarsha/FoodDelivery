using OrderService.Application.DTOs.Order;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Services
{
    public class RestaurantOrderService : IRestaurantOrderService
    {
        private readonly IRestaurantOrderRepository _restaurantOrderRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusPublisher _orderStatusPublisher;

        public RestaurantOrderService(
            IRestaurantOrderRepository restaurantOrderRepository,
            IOrderRepository orderRepository,
            IOrderStatusPublisher orderStatusPublisher)
        {
            _restaurantOrderRepository = restaurantOrderRepository;
            _orderRepository = orderRepository;
            _orderStatusPublisher = orderStatusPublisher;
        }

        public async Task<IEnumerable<PartnerOrderResponseDTO>> GetOrdersForRestaurantAsync(Guid restaurantId)
        {
            var subOrders = await _restaurantOrderRepository.GetByRestaurantId(restaurantId);
            return subOrders.Select(MapToPartnerResponse);
        }

        public async Task<PartnerOrderResponseDTO> GetSubOrderByIdAsync(Guid subOrderId, Guid restaurantId)
        {
            var subOrder = await _restaurantOrderRepository.GetByIdWithItems(subOrderId);
            if (subOrder == null)
                throw new NotFoundException("RestaurantOrder", subOrderId);

            // Security check: ensure this sub-order belongs to the calling partner's restaurant
            if (subOrder.RestaurantId != restaurantId)
                throw new ForbiddenException("You do not have access to this order.");

            return MapToPartnerResponse(subOrder);
        }

        public async Task<PartnerOrderResponseDTO> UpdateSubOrderStatusAsync(
            Guid subOrderId, Guid restaurantId, UpdateRestaurantOrderStatusDTO dto)
        {
            var subOrder = await _restaurantOrderRepository.GetByIdWithItems(subOrderId);
            if (subOrder == null)
                throw new NotFoundException("RestaurantOrder", subOrderId);

            // Security check
            if (subOrder.RestaurantId != restaurantId)
                throw new ForbiddenException("You do not have access to this order.");

            // Validate the status transition
            if (!IsValidPartnerTransition(subOrder.Status, dto.Status))
                throw new BadRequestException(
                    $"Cannot transition from '{subOrder.Status}' to '{dto.Status}'.");

            subOrder.Status = dto.Status;
            subOrder.UpdatedAt = DateTime.UtcNow;

            if (dto.Status == RestaurantOrderStatus.Rejected ||
                dto.Status == RestaurantOrderStatus.Cancelled)
            {
                if (string.IsNullOrWhiteSpace(dto.CancellationReason))
                    throw new BadRequestException("A cancellation/rejection reason is required.");

                subOrder.CancellationReason = dto.CancellationReason;
            }

            await _restaurantOrderRepository.UpdateAsync(subOrder);

            // Sync parent order status
            await SyncParentOrderStatusAsync(subOrder.OrderId);

            return MapToPartnerResponse(subOrder);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Partner status transition rules
        // ─────────────────────────────────────────────────────────────────────
        private static bool IsValidPartnerTransition(RestaurantOrderStatus from, RestaurantOrderStatus to)
        {
            return (from, to) switch
            {
                (RestaurantOrderStatus.Pending, RestaurantOrderStatus.Accepted) => true,
                (RestaurantOrderStatus.Pending, RestaurantOrderStatus.Rejected) => true,
                (RestaurantOrderStatus.Accepted, RestaurantOrderStatus.Preparing) => true,
                (RestaurantOrderStatus.Accepted, RestaurantOrderStatus.Cancelled) => true,
                (RestaurantOrderStatus.Preparing, RestaurantOrderStatus.ReadyForPickup) => true,
                (RestaurantOrderStatus.Preparing, RestaurantOrderStatus.Cancelled) => true,
                _ => false
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // Sync parent Order status based on all sub-orders' current statuses
        // ─────────────────────────────────────────────────────────────────────
        private async Task SyncParentOrderStatusAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdWithDetails(orderId);
            if (order == null) return;

            var subOrders = order.RestaurantOrders.ToList();

            bool allCancelled = subOrders.All(ro =>
                ro.Status == RestaurantOrderStatus.Cancelled ||
                ro.Status == RestaurantOrderStatus.Rejected);

            bool anyAccepted = subOrders.Any(ro =>
                ro.Status == RestaurantOrderStatus.Accepted ||
                ro.Status == RestaurantOrderStatus.Preparing ||
                ro.Status == RestaurantOrderStatus.ReadyForPickup ||
                ro.Status == RestaurantOrderStatus.PickedUp);

            if (allCancelled)
                order.Status = OrderStatus.CancelledBySystem;
            else if (anyAccepted && order.Status == OrderStatus.Placed)
                order.Status = OrderStatus.InProgress;

            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Mapping
        // ─────────────────────────────────────────────────────────────────────
        private static PartnerOrderResponseDTO MapToPartnerResponse(Domain.Entities.RestaurantOrder ro)
        {
            var parent = ro.Order;
            return new PartnerOrderResponseDTO
            {
                SubOrderId = ro.Id,
                ParentOrderId = ro.OrderId,
                Status = ro.Status.ToString(),
                SubTotal = ro.SubTotal,
                CancellationReason = ro.CancellationReason,
                CreatedAt = ro.CreatedAt,
                UpdatedAt = ro.UpdatedAt,
                CustomerName = parent?.CustomerName ?? string.Empty,
                DeliveryStreet = parent?.Street ?? string.Empty,
                DeliveryCity = parent?.City ?? string.Empty,
                DeliveryPincode = parent?.Pincode ?? string.Empty,
                DeliveryInstructions = parent?.DeliveryInstructions,
                PaymentMethod = parent?.Payment?.Method.ToString() ?? "Unknown",
                PaymentStatus = parent?.Payment?.Status.ToString() ?? "Unknown",
                Items = ro.OrderItems.Select(oi => new OrderItemResponseDTO
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItemName,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };
        }
    }
}
