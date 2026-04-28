using OrderService.Domain.ExternalDTO;
using OrderService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace OrderService.Infrastructure.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly HttpClient _client;

        public CatalogRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<MenuItemDTO?> GetItemById(Guid id)
        {
            var response = await _client.GetAsync($"api/MenuItem/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<MenuItemDTO>();
        }

        public async Task<RestaurantDTO?> GetRestaurantById(Guid id)
        {
            var response = await _client.GetAsync($"api/Restaurant/restaurant/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<RestaurantDTO>();
        }

        public async Task<CategoryDTO?> GetCategoryById(Guid id)
        {
            var response = await _client.GetAsync($"api/Category/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CategoryDTO>();
        }

        public async Task<bool> IsServiceAreaAvailable(Guid restaurantId, string pincode)
        {
            var response = await _client.GetAsync($"api/ServiceArea/{restaurantId}/serviceable/{pincode}");
            if (!response.IsSuccessStatusCode) return false;
            
            var result = await response.Content.ReadFromJsonAsync<ServiceAreaCheckResponse>();
            return result?.IsServiceable ?? false;
        }

        public async Task<IEnumerable<RestaurantDTO>> GetRestaurantsByOwnerId(string ownerId)
        {
            var response = await _client.GetAsync($"api/Restaurant/owner/{ownerId}");
            if (!response.IsSuccessStatusCode) return new List<RestaurantDTO>();
            
            var restaurants = await response.Content.ReadFromJsonAsync<IEnumerable<RestaurantDTO>>();
            return restaurants ?? new List<RestaurantDTO>();
        }

        public async Task<bool> IsRestaurantOwnedByPartner(Guid restaurantId, string partnerId)
        {
            var restaurant = await GetRestaurantById(restaurantId);
            return restaurant != null && restaurant.OwnerId == partnerId;
        }
    }

    // Helper class to deserialize the response
    internal class ServiceAreaCheckResponse
    {
        public bool IsServiceable { get; set; }
    }
}
