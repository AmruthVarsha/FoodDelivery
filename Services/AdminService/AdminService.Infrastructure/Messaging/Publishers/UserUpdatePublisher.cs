using MassTransit;
using Shared.Events;

namespace AdminService.Infrastructure.Messaging.Publishers
{
    public class UserUpdatePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public UserUpdatePublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishUserUpdate(string userId, string fullName, string email, 
            string phoneNo, string role, bool isActive, bool emailConfirmed, 
            bool twoFactorEnabled, string eventType)
        {
            var userEvent = new UserDataSyncEvent
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
            };

            await _publishEndpoint.Publish(userEvent);
        }
    }
}
