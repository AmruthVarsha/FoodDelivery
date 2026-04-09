using AdminService.Application.DTOs.Users;
using AdminService.Application.Exceptions;
using AdminService.Application.Interfaces;
using AdminService.Application.Interfaces.Services;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Messaging.Publishers;

namespace AdminService.Application.Services
{
    public class UserApprovalService : IUserApprovalService
    {
        private readonly IUserRoleApprovalRepository _userRoleApprovalRepository;
        private readonly UserRoleApprovalResponsePublisher _publisher;
        private readonly IEmailService _emailService;

        public UserApprovalService(
            IUserRoleApprovalRepository userRoleApprovalRepository,
            UserRoleApprovalResponsePublisher publisher,
            IEmailService emailService)
        {
            _userRoleApprovalRepository = userRoleApprovalRepository;
            _publisher = publisher;
            _emailService = emailService;
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
            
            // Send approval email to user
            await _emailService.SendEmailAsync(
                email,
                "Account Approved - Food Delivery App",
                $@"<h3>Account Approval Notification</h3>
                   <p>Dear {request.FullName},</p>
                   <p>Congratulations! Your account has been approved by the administrator.</p>
                   <p><strong>Role:</strong> {request.Role}</p>
                   <p>You can now login and start using the Food Delivery App.</p>
                   <br/>
                   <p>Thank you for joining us!</p>
                   <p>Regards,<br/>Food Delivery App Team</p>"
            );
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
            
            // Send rejection email to user
            await _emailService.SendEmailAsync(
                email,
                "Account Registration Update - Food Delivery App",
                $@"<h3>Account Registration Update</h3>
                   <p>Dear {request.FullName},</p>
                   <p>We regret to inform you that your account registration request has been reviewed and not approved at this time.</p>
                   <p><strong>Reason:</strong> {dto.Reason}</p>
                   <p>If you have any questions or would like to reapply, please contact our support team.</p>
                   <br/>
                   <p>Regards,<br/>Food Delivery App Team</p>"
            );
        }
    }
}
