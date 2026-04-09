using MassTransit;
using Shared.Events;
using AdminService.Domain.Interfaces;
using AdminService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AdminService.Infrastructure.Messaging.Consumers
{
    public class UserRoleApprovalConsumer : IConsumer<UserRoleApprovalRequestEvent>
    {
        private readonly IUserRoleApprovalRepository _userRoleApprovalRepository;
        private readonly ILogger<UserRoleApprovalConsumer> _logger;

        public UserRoleApprovalConsumer(
            IUserRoleApprovalRepository userRoleApprovalRepository,
            ILogger<UserRoleApprovalConsumer> logger)
        {
            _userRoleApprovalRepository = userRoleApprovalRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserRoleApprovalRequestEvent> context)
        {
            try
            {
                var message = context.Message;
                _logger.LogInformation("Received UserRoleApprovalRequest for email: {Email}, Role: {Role}", message.Email, message.Role);

                var existing = await _userRoleApprovalRepository.GetByEmailAsync(message.Email);

                if (existing == null)
                {
                    await _userRoleApprovalRepository.AddAsync(new UserRoleApprovalRequest
                    {
                        Id = Guid.NewGuid(),
                        Email = message.Email,
                        FullName = message.FullName,
                        Role = message.Role,
                        RequestedAt = message.RequestedAt,
                        IsApproved = false
                    });
                    _logger.LogInformation("Successfully added UserRoleApprovalRequest for email: {Email}", message.Email);
                }
                else
                {
                    _logger.LogInformation("UserRoleApprovalRequest already exists for email: {Email}", message.Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UserRoleApprovalRequest for email: {Email}", context.Message.Email);
                throw;
            }
        }
    }
}
