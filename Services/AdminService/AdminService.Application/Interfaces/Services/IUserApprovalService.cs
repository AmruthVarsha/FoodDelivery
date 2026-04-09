using AdminService.Application.DTOs.Users;

namespace AdminService.Application.Interfaces.Services
{
    public interface IUserApprovalService
    {
        Task<List<UserRoleApprovalDto>> GetPendingUserApprovalsAsync();
        Task ApproveUserRoleAsync(string email, string adminId, ApproveRejectDto dto);
        Task RejectUserRoleAsync(string email, string adminId, ApproveRejectDto dto);
    }
}
