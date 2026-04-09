using MassTransit;
using Shared.Events;

namespace AdminService.Infrastructure.Messaging.Publishers
{
    public class RestaurantApprovalResponsePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RestaurantApprovalResponsePublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishApprovalResponse(Guid restaurantId, bool isApproved, string? adminId, string? rejectionReason)
        {
            await _publishEndpoint.Publish(new RestaurantApprovalResponseEvent
            {
                RestaurantId = restaurantId,
                IsApproved = isApproved,
                ApprovedByAdminId = adminId,
                RejectionReason = rejectionReason,
                ProcessedAt = DateTime.UtcNow
            });
        }
    }
}
