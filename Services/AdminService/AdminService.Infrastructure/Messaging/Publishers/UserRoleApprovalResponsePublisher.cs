using MassTransit;
using Shared.Events;

namespace AdminService.Infrastructure.Messaging.Publishers
{
    public class UserRoleApprovalResponsePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public UserRoleApprovalResponsePublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishApprovalResponse(string email, bool isApproved, string? adminId, string? rejectionReason)
        {
            await _publishEndpoint.Publish(new UserRoleApprovalResponseEvent
            {
                Email = email,
                IsApproved = isApproved,
                ApprovedByAdminId = adminId,
                RejectionReason = rejectionReason,
                ProcessedAt = DateTime.UtcNow
            });
        }
    }
}
