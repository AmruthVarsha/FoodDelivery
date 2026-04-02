using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<IEnumerable<Category>> GetByRestaurantIdAsync(Guid restaurantId);
        public Task<Category?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        public Task<Guid> CreateAsync(Category category);
        public Task UpdateAsync(Category category);
        public Task DeleteAsync(Guid id);
    }
}