using MassTransit;
using Shared.Events;

namespace CatalogService.Infrastructure.Messaging.Publishers
{
    public class RestaurantDataSyncPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RestaurantDataSyncPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishRestaurantDataSync(Guid restaurantId, string ownerId, string name, string email, 
            string phoneNumber, double rating, int totalRatings, bool isActive, bool isApproved, string eventType)
        {
            await _publishEndpoint.Publish(new RestaurantDataSyncEvent
            {
                RestaurantId = restaurantId,
                OwnerId = ownerId,
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                Rating = rating,
                TotalRatings = totalRatings,
                IsActive = isActive,
                IsApproved = isApproved,
                EventType = eventType,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
