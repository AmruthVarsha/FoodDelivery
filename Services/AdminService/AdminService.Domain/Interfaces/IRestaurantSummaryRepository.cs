using AdminService.Domain.Entities;

namespace AdminService.Domain.Interfaces
{
    public interface IRestaurantSummaryRepository
    {
        Task AddAsync(RestaurantSummary restaurant);
        Task<List<RestaurantSummary>> GetAllAsync();
        Task<RestaurantSummary?> GetByRestaurantIdAsync(Guid restaurantId);
        Task UpdateAsync(RestaurantSummary restaurant);
        Task DeleteAsync(Guid restaurantId);
        Task<List<RestaurantSummary>> GetByOwnerIdAsync(string ownerId);
        Task<int> GetTotalRestaurantsCountAsync();
        Task<int> GetActiveRestaurantsCountAsync();
    }
}
