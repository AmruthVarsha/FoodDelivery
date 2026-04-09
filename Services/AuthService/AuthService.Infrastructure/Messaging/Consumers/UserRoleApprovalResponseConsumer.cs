
using MassTransit;
using Shared.Events;
using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Messaging.Consumers
{
    public class UserRoleApprovalResponseConsumer : IConsumer<UserRoleApprovalResponseEvent>
    {
        private readonly IAuthRepository _authRepository;

        public UserRoleApprovalResponseConsumer(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task Consume(ConsumeContext<UserRoleApprovalResponseEvent> context)
        {
            var message = context.Message;

            if (message.IsApproved)
            {
                // Find user by email
                var user = await _authRepository.FindByEmailAsync(message.Email);
                if (user == null)
                {
                    return;
                }

                // Activate the account (IsActive = true)
                await _authRepository.ChangeAccountStatusAsync(user.Id, true);

                // Mark the approval request as approved in AuthService
                await _authRepository.ApproveRequest(message.Email);
            }
        }
    }
}
