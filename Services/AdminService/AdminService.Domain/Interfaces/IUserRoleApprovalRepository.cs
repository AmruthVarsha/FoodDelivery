using AdminService.Domain.Entities;

namespace AdminService.Domain.Interfaces
{
    public interface IUserRoleApprovalRepository
    {
        Task AddAsync(UserRoleApprovalRequest request);
        Task<List<UserRoleApprovalRequest>> GetPendingRequestsAsync();
        Task<UserRoleApprovalRequest?> GetByEmailAsync(string email);
        Task ApproveAsync(string email, string adminId);
        Task RejectAsync(string email, string adminId, string reason);
    }
}
