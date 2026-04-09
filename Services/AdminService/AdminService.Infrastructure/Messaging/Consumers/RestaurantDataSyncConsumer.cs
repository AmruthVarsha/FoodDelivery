using MassTransit;
using Shared.Events;
using AdminService.Domain.Interfaces;
using AdminService.Domain.Entities;

namespace AdminService.Infrastructure.Messaging.Consumers
{
    public class RestaurantDataSyncConsumer : IConsumer<RestaurantDataSyncEvent>
    {
        private readonly IRestaurantSummaryRepository _restaurantSummaryRepository;

        public RestaurantDataSyncConsumer(IRestaurantSummaryRepository restaurantSummaryRepository)
        {
            _restaurantSummaryRepository = restaurantSummaryRepository;
        }

        public async Task Consume(ConsumeContext<RestaurantDataSyncEvent> context)
        {
            var message = context.Message;

            var existing = await _restaurantSummaryRepository.GetByRestaurantIdAsync(message.RestaurantId);

            if (message.EventType == "Created" && existing == null)
            {
                await _restaurantSummaryRepository.AddAsync(new RestaurantSummary
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = message.RestaurantId,
                    OwnerId = message.OwnerId,
                    Name = message.Name,
                    Email = message.Email,
                    PhoneNumber = message.PhoneNumber,
                    Rating = message.Rating,
                    TotalRatings = message.TotalRatings,
                    IsActive = message.IsActive,
                    IsApproved = message.IsApproved,
                    CreatedAt = message.Timestamp,
                    LastUpdatedAt = message.Timestamp
                });
            }
            else if (message.EventType == "Updated" && existing != null)
            {
                existing.Name = message.Name;
                existing.Email = message.Email;
                existing.PhoneNumber = message.PhoneNumber;
                existing.Rating = message.Rating;
                existing.TotalRatings = message.TotalRatings;
                existing.IsActive = message.IsActive;
                existing.IsApproved = message.IsApproved;
                existing.LastUpdatedAt = message.Timestamp;

                await _restaurantSummaryRepository.UpdateAsync(existing);
            }
            else if (message.EventType == "Deleted" && existing != null)
            {
                await _restaurantSummaryRepository.DeleteAsync(message.RestaurantId);
            }
        }
    }
}
