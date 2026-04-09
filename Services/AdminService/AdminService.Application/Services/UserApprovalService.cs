using AdminService.Application.DTOs.Users;
using AdminService.Application.Exceptions;
using AdminService.Application.Interfaces.Services;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Messaging.Publishers;

namespace AdminService.Application.Services
{
    public class UserApprovalService : IUserApprovalService
    {
        private readonly IUserRoleApprovalRepository _userRoleApprovalRepository;
        private readonly UserRoleApprovalResponsePublisher _publisher;

        public UserApprovalService(
            IUserRoleApprovalRepository userRoleApprovalRepository,
            UserRoleApprovalResponsePublisher publisher)
        {
            _userRoleApprovalRepository = userRoleApprovalRepository;
            _publisher = publisher;
        }

        public async Task<List<UserRoleApprovalDto>> GetPendingUserApprovalsAsync()
        {
            var requests = await _userRoleApprovalRepository.GetPendingRequestsAsync();
            return requests.Select(r => new UserRoleApprovalDto
            {
                Email = r.Email,
                FullName = r.FullName,
                Role = r.Role,
                RequestedAt = r.RequestedAt
            }).ToList();
        }

        public async Task ApproveUserRoleAsync(string email, string adminId, ApproveRejectDto dto)
        {
            var request = await _userRoleApprovalRepository.GetByEmailAsync(email);
            if (request == null)
                throw new NotFoundException($"Approval request for {email} not found");

            await _userRoleApprovalRepository.ApproveAsync(email, adminId);
            await _publisher.PublishApprovalResponse(email, true, adminId, null);
        }

        public async Task RejectUserRoleAsync(string email, string adminId, ApproveRejectDto dto)
        {
            var request = await _userRoleApprovalRepository.GetByEmailAsync(email);
            if (request == null)
                throw new NotFoundException($"Approval request for {email} not found");

            if (string.IsNullOrEmpty(dto.Reason))
                throw new BadRequestException("Rejection reason is required");

            await _userRoleApprovalRepository.RejectAsync(email, adminId, dto.Reason);
            await _publisher.PublishApprovalResponse(email, false, adminId, dto.Reason);
        }
    }
}
