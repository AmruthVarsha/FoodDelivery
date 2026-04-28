using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories
{
    public class DeliveryAgentProfileRepository : IDeliveryAgentProfileRepository
    {
        private readonly OrderDbContext _context;

        public DeliveryAgentProfileRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryAgentProfile?> GetByAgentUserId(string agentUserId)
        {
            return await _context.DeliveryAgentProfiles
                .FirstOrDefaultAsync(p => p.AgentUserId == agentUserId);
        }

        public async Task<IEnumerable<DeliveryAgentProfile>> GetActiveByPincode(string pincode)
        {
            return await _context.DeliveryAgentProfiles
                .Where(p => p.IsActive && p.CurrentPincode == pincode)
                .OrderBy(p => p.LastUpdated) // oldest "last seen" = has been idle longest = most available
                .ToListAsync();
        }

        public async Task AddAsync(DeliveryAgentProfile profile)
        {
            _context.DeliveryAgentProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DeliveryAgentProfile profile)
        {
            _context.DeliveryAgentProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }
    }
}
