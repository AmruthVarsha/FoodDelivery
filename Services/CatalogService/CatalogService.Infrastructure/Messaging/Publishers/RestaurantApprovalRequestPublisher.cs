using MassTransit;
using Shared.Events;

namespace CatalogService.Infrastructure.Messaging.Publishers
{
    public class RestaurantApprovalRequestPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RestaurantApprovalRequestPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishApprovalRequest(Guid restaurantId, string ownerId, string restaurantName, string email, string phoneNumber)
        {
            await _publishEndpoint.Publish(new RestaurantApprovalRequestEvent
            {
                RestaurantId = restaurantId,
                OwnerId = ownerId,
                RestaurantName = restaurantName,
                Email = email,
                PhoneNumber = phoneNumber,
                RequestedAt = DateTime.UtcNow
            });
        }
    }
}
