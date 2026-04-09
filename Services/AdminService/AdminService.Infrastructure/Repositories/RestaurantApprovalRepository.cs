using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Repositories
{
    public class RestaurantApprovalRepository : IRestaurantApprovalRepository
    {
        private readonly AdminDbContext _context;

        public RestaurantApprovalRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RestaurantApprovalRequest request)
        {
            await _context.RestaurantApprovalRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RestaurantApprovalRequest>> GetPendingRequestsAsync()
        {
            return await _context.RestaurantApprovalRequests
                .Where(r => !r.IsApproved && r.RejectionReason == null)
                .ToListAsync();
        }

        public async Task<RestaurantApprovalRequest?> GetByRestaurantIdAsync(Guid restaurantId)
        {
            return await _context.RestaurantApprovalRequests
                .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
        }

        public async Task ApproveAsync(Guid restaurantId, string adminId)
        {
            var request = await _context.RestaurantApprovalRequests
                .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
            
            if (request != null)
            {
                request.IsApproved = true;
                request.ApprovedAt = DateTime.UtcNow;
                request.ApprovedByAdminId = adminId;
                _context.RestaurantApprovalRequests.Update(request);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectAsync(Guid restaurantId, string adminId, string reason)
        {
            var request = await _context.RestaurantApprovalRequests
                .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
            
            if (request != null)
            {
                request.IsApproved = false;
                request.ApprovedAt = DateTime.UtcNow;
                request.ApprovedByAdminId = adminId;
                request.RejectionReason = reason;
                _context.RestaurantApprovalRequests.Update(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}
