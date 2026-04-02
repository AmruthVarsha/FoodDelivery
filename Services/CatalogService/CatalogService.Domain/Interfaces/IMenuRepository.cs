using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Interfaces
{
    public interface IMenuItemRepository
    {
        public Task<IEnumerable<MenuItem>> GetByCategoryIdAsync(Guid categoryId);
        public Task<IEnumerable<MenuItem>> GetByRestaurantIdAsync(Guid restaurantId);
        public Task<MenuItem?> GetByIdAsync(Guid id);
        public Task<bool> ExistsAsync(Guid id);
        public Task<Guid> CreateAsync(MenuItem menuItem);
        public Task UpdateAsync(MenuItem menuItem);
        public Task DeleteAsync(Guid id);
    }
}