using OrderService.Domain.ExternalDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Domain.Interfaces
{
    public interface ICatalogRepository
    {
        Task<MenuItemDTO?> GetItemById(Guid id);
        Task<RestaurantDTO?> GetRestaurantById(Guid id);
        Task<CategoryDTO?> GetCategoryById(Guid id);
        Task<bool> IsServiceAreaAvailable(Guid restaurantId, string pincode);
        Task<IEnumerable<RestaurantDTO>> GetRestaurantsByOwnerId(string ownerId);
    }
}
