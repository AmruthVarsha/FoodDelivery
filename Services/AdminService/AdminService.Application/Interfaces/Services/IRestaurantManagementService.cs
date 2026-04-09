using AdminService.Application.DTOs.Restaurants;

namespace AdminService.Application.Interfaces.Services
{
    public interface IRestaurantManagementService
    {
        Task<List<RestaurantSummaryDto>> GetAllRestaurantsAsync();
        Task<RestaurantSummaryDto> GetRestaurantByIdAsync(Guid restaurantId);
        Task<List<RestaurantSummaryDto>> GetRestaurantsByOwnerAsync(string ownerId);
        Task DeleteRestaurantAsync(Guid restaurantId);
    }
}
