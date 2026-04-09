using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Repositories
{
    public class UserRoleApprovalRepository : IUserRoleApprovalRepository
    {
        private readonly AdminDbContext _context;

        public UserRoleApprovalRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserRoleApprovalRequest request)
        {
            await _context.UserRoleApprovalRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserRoleApprovalRequest>> GetPendingRequestsAsync()
        {
            return await _context.UserRoleApprovalRequests
                .Where(r => !r.IsApproved && r.RejectionReason == null)
                .ToListAsync();
        }

        public async Task<UserRoleApprovalRequest?> GetByEmailAsync(string email)
        {
            return await _context.UserRoleApprovalRequests
                .FirstOrDefaultAsync(r => r.Email == email);
        }

        public async Task ApproveAsync(string email, string adminId)
        {
            var request = await _context.UserRoleApprovalRequests
                .FirstOrDefaultAsync(r => r.Email == email);
            
            if (request != null)
            {
                request.IsApproved = true;
                request.ApprovedAt = DateTime.UtcNow;
                request.ApprovedByAdminId = adminId;
                _context.UserRoleApprovalRequests.Update(request);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectAsync(string email, string adminId, string reason)
        {
            var request = await _context.UserRoleApprovalRequests
                .FirstOrDefaultAsync(r => r.Email == email);
            
            if (request != null)
            {
                request.IsApproved = false;
                request.ApprovedAt = DateTime.UtcNow;
                request.ApprovedByAdminId = adminId;
                request.RejectionReason = reason;
                _context.UserRoleApprovalRequests.Update(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}
