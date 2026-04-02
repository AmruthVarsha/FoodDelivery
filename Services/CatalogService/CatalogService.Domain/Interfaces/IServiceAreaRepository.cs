using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Interfaces
{
    public interface IServiceAreaRepository
    {
        public Task<IEnumerable<ServiceArea>> GetByRestaurantIdAsync(Guid restaurantId);
        public Task<ServiceArea?> GetByIdAsync(Guid id);
        public Task<bool> IsPincodeServiceableAsync(Guid restaurantId, string pincode);
        public Task<Guid> CreateAsync(ServiceArea serviceArea);
        public Task DeleteAsync(Guid id);
    }
}