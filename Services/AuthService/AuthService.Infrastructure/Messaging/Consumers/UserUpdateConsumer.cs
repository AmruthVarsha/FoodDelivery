using MassTransit;
using Shared.Events;
using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Messaging.Consumers
{
    public class UserUpdateConsumer : IConsumer<UserDataSyncEvent>
    {
        private readonly IUserRepository _userRepository;

        public UserUpdateConsumer(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<UserDataSyncEvent> context)
        {
            var message = context.Message;

            if (message.EventType == "Updated")
            {
                var user = await _userRepository.GetByIdAsync(message.UserId);
                if (user != null)
                {
                    user.FullName = message.FullName;
                    user.PhoneNo = message.PhoneNo;
                    user.IsActive = message.IsActive;
                    user.EmailConfirmed = message.EmailConfirmed;
                    user.TwoFactorEnabled = message.TwoFactorEnabled;

                    await _userRepository.UpdateAsync(user);
                }
            }
            else if (message.EventType == "Deleted")
            {
                var user = await _userRepository.GetByIdAsync(message.UserId);
                if (user != null)
                {
                    user.IsActive = false;
                    await _userRepository.UpdateAsync(user);
                }
            }
        }
    }
}
