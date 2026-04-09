using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Repositories
{
    public class RestaurantSummaryRepository : IRestaurantSummaryRepository
    {
        private readonly AdminDbContext _context;

        public RestaurantSummaryRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RestaurantSummary restaurant)
        {
            await _context.RestaurantSummaries.AddAsync(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RestaurantSummary>> GetAllAsync()
        {
            return await _context.RestaurantSummaries.ToListAsync();
        }

        public async Task<RestaurantSummary?> GetByRestaurantIdAsync(Guid restaurantId)
        {
            return await _context.RestaurantSummaries
                .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
        }

        public async Task UpdateAsync(RestaurantSummary restaurant)
        {
            _context.RestaurantSummaries.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid restaurantId)
        {
            var restaurant = await _context.RestaurantSummaries
                .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
            
            if (restaurant != null)
            {
                _context.RestaurantSummaries.Remove(restaurant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<RestaurantSummary>> GetByOwnerIdAsync(string ownerId)
        {
            return await _context.RestaurantSummaries
                .Where(r => r.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<int> GetTotalRestaurantsCountAsync()
        {
            return await _context.RestaurantSummaries.CountAsync();
        }

        public async Task<int> GetActiveRestaurantsCountAsync()
        {
            return await _context.RestaurantSummaries.CountAsync(r => r.IsActive);
        }
    }
}
