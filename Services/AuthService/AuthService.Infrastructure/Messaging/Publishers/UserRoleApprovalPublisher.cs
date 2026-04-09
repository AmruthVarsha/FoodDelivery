using MassTransit;
using Shared.Events;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Messaging.Publishers
{
    public class UserRoleApprovalPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UserRoleApprovalPublisher> _logger;

        public UserRoleApprovalPublisher(
            IPublishEndpoint publishEndpoint,
            ILogger<UserRoleApprovalPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishApprovalRequest(string email, string fullName, string role)
        {
            try
            {
                _logger.LogInformation("Publishing UserRoleApprovalRequest for email: {Email}, Role: {Role}", email, role);
                
                await _publishEndpoint.Publish(new UserRoleApprovalRequestEvent
                {
                    Email = email,
                    FullName = fullName,
                    Role = role,
                    RequestedAt = DateTime.UtcNow
                });
                
                _logger.LogInformation("Successfully published UserRoleApprovalRequest for email: {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing UserRoleApprovalRequest for email: {Email}", email);
                throw;
            }
        }
    }
}
