using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly CatalogDbContext _context;
        public MenuItemRepository(CatalogDbContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _context.MenuItems
                .Include(mi => mi.Category)
                .Where(mi => mi.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetByRestaurantIdAsync(Guid restaurantId)
        {
            return await _context.MenuItems
                .Include(mi => mi.Category)
                .Where(mi => mi.RestaurantId == restaurantId)
                .ToListAsync();
        }
        public async Task<MenuItem?> GetByIdAsync(Guid id)
        {
            return await _context.MenuItems
                .Include(mi => mi.Category)
                .FirstOrDefaultAsync(mi => mi.Id == id);
        }

        public async Task<IEnumerable<MenuItem>> SearchByNameAsync(string searchTerm)
        {
            return await _context.MenuItems
                .Include(mi => mi.Category)
                .Where(mi => mi.Name.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.MenuItems.AnyAsync(mi => mi.Id == id);
        }
        public async Task<Guid> CreateAsync(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            return menuItem.Id;
        }
        public async Task UpdateAsync(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem is null) return;
            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBulkAsync(IEnumerable<MenuItem> menuItems)
        {
            _context.MenuItems.UpdateRange(menuItems);
            await _context.SaveChangesAsync();
        }
    }
}
