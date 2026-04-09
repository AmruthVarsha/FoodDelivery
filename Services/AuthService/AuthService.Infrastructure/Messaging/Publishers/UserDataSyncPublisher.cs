using MassTransit;
using Shared.Events;

namespace AuthService.Infrastructure.Messaging.Publishers
{
    public class UserDataSyncPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public UserDataSyncPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishUserDataSync(string userId, string fullName, string email, string phoneNo, 
            string role, bool isActive, bool emailConfirmed, bool twoFactorEnabled, string eventType)
        {
            await _publishEndpoint.Publish(new UserDataSyncEvent
            {
                UserId = userId,
                FullName = fullName,
                Email = email,
                PhoneNo = phoneNo,
                Role = role,
                IsActive = isActive,
                EmailConfirmed = emailConfirmed,
                TwoFactorEnabled = twoFactorEnabled,
                EventType = eventType,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
