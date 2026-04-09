using MassTransit;
using Shared.Events;
using CatalogService.Domain.Interfaces;

namespace CatalogService.Infrastructure.Messaging.Consumers
{
    public class RestaurantApprovalResponseConsumer : IConsumer<RestaurantApprovalResponseEvent>
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantApprovalResponseConsumer(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public async Task Consume(ConsumeContext<RestaurantApprovalResponseEvent> context)
        {
            var message = context.Message;

            var restaurant = await _restaurantRepository.GetByIdAsync(message.RestaurantId);
            if (restaurant != null)
            {
                restaurant.IsApproved = message.IsApproved;
                await _restaurantRepository.UpdateAsync(restaurant);
            }
        }
    }
}
