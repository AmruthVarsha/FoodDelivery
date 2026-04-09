using MassTransit;
using Shared.Events;

namespace AdminService.Infrastructure.Messaging.Publishers
{
    public class RestaurantUpdatePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RestaurantUpdatePublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishRestaurantUpdate(Guid restaurantId, string ownerId, string name, 
            string email, string phoneNumber, double rating, int totalRatings, 
            bool isActive, bool isApproved, string eventType)
        {
            var restaurantEvent = new RestaurantDataSyncEvent
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
            };

            await _publishEndpoint.Publish(restaurantEvent);
        }
    }
}
