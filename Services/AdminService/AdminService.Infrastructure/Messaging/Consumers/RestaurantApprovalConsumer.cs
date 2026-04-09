using MassTransit;
using Shared.Events;
using AdminService.Domain.Interfaces;
using AdminService.Domain.Entities;

namespace AdminService.Infrastructure.Messaging.Consumers
{
    public class RestaurantApprovalConsumer : IConsumer<RestaurantApprovalRequestEvent>
    {
        private readonly IRestaurantApprovalRepository _restaurantApprovalRepository;

        public RestaurantApprovalConsumer(IRestaurantApprovalRepository restaurantApprovalRepository)
        {
            _restaurantApprovalRepository = restaurantApprovalRepository;
        }

        public async Task Consume(ConsumeContext<RestaurantApprovalRequestEvent> context)
        {
            var message = context.Message;

            var existing = await _restaurantApprovalRepository.GetByRestaurantIdAsync(message.RestaurantId);

            if (existing == null)
            {
                await _restaurantApprovalRepository.AddAsync(new RestaurantApprovalRequest
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = message.RestaurantId,
                    OwnerId = message.OwnerId,
                    RestaurantName = message.RestaurantName,
                    Email = message.Email,
                    PhoneNumber = message.PhoneNumber,
                    RequestedAt = message.RequestedAt,
                    IsApproved = false
                });
            }
        }
    }
}
