using AdminService.Application.DTOs.Restaurants;
using AdminService.Application.Exceptions;
using AdminService.Application.Interfaces.Services;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Messaging.Publishers;

namespace AdminService.Application.Services
{
    public class RestaurantManagementService : IRestaurantManagementService
    {
        private readonly IRestaurantSummaryRepository _restaurantSummaryRepository;
        private readonly RestaurantUpdatePublisher _restaurantUpdatePublisher;

        public RestaurantManagementService(
            IRestaurantSummaryRepository restaurantSummaryRepository,
            RestaurantUpdatePublisher restaurantUpdatePublisher)
        {
            _restaurantSummaryRepository = restaurantSummaryRepository;
            _restaurantUpdatePublisher = restaurantUpdatePublisher;
        }

        public async Task<List<RestaurantSummaryDto>> GetAllRestaurantsAsync()
        {
            var restaurants = await _restaurantSummaryRepository.GetAllAsync();
            return restaurants.Select(r => MapToDto(r)).ToList();
        }

        public async Task<RestaurantSummaryDto> GetRestaurantByIdAsync(Guid restaurantId)
        {
            var restaurant = await _restaurantSummaryRepository.GetByRestaurantIdAsync(restaurantId);
            if (restaurant == null)
                throw new NotFoundException("Restaurant", restaurantId);

            return MapToDto(restaurant);
        }

        public async Task<List<RestaurantSummaryDto>> GetRestaurantsByOwnerAsync(string ownerId)
        {
            var restaurants = await _restaurantSummaryRepository.GetByOwnerIdAsync(ownerId);
            return restaurants.Select(r => MapToDto(r)).ToList();
        }

        public async Task DeleteRestaurantAsync(Guid restaurantId)
        {
            var restaurant = await _restaurantSummaryRepository.GetByRestaurantIdAsync(restaurantId);
            if (restaurant == null)
                throw new NotFoundException("Restaurant", restaurantId);

            await _restaurantSummaryRepository.DeleteAsync(restaurantId);

            await _restaurantUpdatePublisher.PublishRestaurantUpdate(
                restaurant.RestaurantId, restaurant.OwnerId, restaurant.Name,
                restaurant.Email, restaurant.PhoneNumber, restaurant.Rating,
                restaurant.TotalRatings, false, restaurant.IsApproved, "Deleted");
        }

        private static RestaurantSummaryDto MapToDto(Domain.Entities.RestaurantSummary restaurant)
        {
            return new RestaurantSummaryDto
            {
                RestaurantId = restaurant.RestaurantId,
                OwnerId = restaurant.OwnerId,
                Name = restaurant.Name,
                Email = restaurant.Email,
                PhoneNumber = restaurant.PhoneNumber,
                Rating = restaurant.Rating,
                TotalRatings = restaurant.TotalRatings,
                IsActive = restaurant.IsActive,
                IsApproved = restaurant.IsApproved,
                CreatedAt = restaurant.CreatedAt,
                LastUpdatedAt = restaurant.LastUpdatedAt
            };
        }
    }
}
