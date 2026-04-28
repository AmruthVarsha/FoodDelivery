using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories
{
    public class RestaurantOrderRepository : IRestaurantOrderRepository
    {
        private readonly OrderDbContext _context;

        public RestaurantOrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<RestaurantOrder?> GetById(Guid id)
        {
            return await _context.RestaurantOrders.FindAsync(id);
        }

        public async Task<RestaurantOrder?> GetByIdWithItems(Guid id)
        {
            return await _context.RestaurantOrders
                .Include(ro => ro.OrderItems)
                .Include(ro => ro.Order) // include parent for address/customer info
                .FirstOrDefaultAsync(ro => ro.Id == id);
        }

        public async Task<IEnumerable<RestaurantOrder>> GetByOrderId(Guid orderId)
        {
            return await _context.RestaurantOrders
                .Include(ro => ro.OrderItems)
                .Where(ro => ro.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RestaurantOrder>> GetByRestaurantId(Guid restaurantId)
        {
            return await _context.RestaurantOrders
                .Include(ro => ro.OrderItems)
                .Include(ro => ro.Order) // partner needs customer address
                .Where(ro => ro.RestaurantId == restaurantId)
                .OrderByDescending(ro => ro.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(RestaurantOrder restaurantOrder)
        {
            _context.RestaurantOrders.Add(restaurantOrder);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RestaurantOrder restaurantOrder)
        {
            _context.RestaurantOrders.Update(restaurantOrder);
            await _context.SaveChangesAsync();
        }
    }
}
