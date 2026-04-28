using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces
{
    public interface IRestaurantOrderRepository
    {
        Task<RestaurantOrder?> GetById(Guid id);
        Task<RestaurantOrder?> GetByIdWithItems(Guid id);
        Task<IEnumerable<RestaurantOrder>> GetByOrderId(Guid orderId);
        Task<IEnumerable<RestaurantOrder>> GetByRestaurantId(Guid restaurantId);
        Task AddAsync(RestaurantOrder restaurantOrder);
        Task UpdateAsync(RestaurantOrder restaurantOrder);
    }
}
