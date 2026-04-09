using MassTransit;
using Shared.Events;
using AdminService.Domain.Interfaces;
using AdminService.Domain.Entities;

namespace AdminService.Infrastructure.Messaging.Consumers
{
    public class UserDataSyncConsumer : IConsumer<UserDataSyncEvent>
    {
        private readonly IUserSummaryRepository _userSummaryRepository;

        public UserDataSyncConsumer(IUserSummaryRepository userSummaryRepository)
        {
            _userSummaryRepository = userSummaryRepository;
        }

        public async Task Consume(ConsumeContext<UserDataSyncEvent> context)
        {
            var message = context.Message;

            var existing = await _userSummaryRepository.GetByUserIdAsync(message.UserId);

            if (message.EventType == "Created" && existing == null)
            {
                await _userSummaryRepository.AddAsync(new UserSummary
                {
                    Id = Guid.NewGuid(),
                    UserId = message.UserId,
                    FullName = message.FullName,
                    Email = message.Email,
                    PhoneNo = message.PhoneNo,
                    Role = message.Role,
                    IsActive = message.IsActive,
                    EmailConfirmed = message.EmailConfirmed,
                    TwoFactorEnabled = message.TwoFactorEnabled,
                    CreatedAt = message.Timestamp,
                    LastUpdatedAt = message.Timestamp
                });
            }
            else if (message.EventType == "Updated" && existing != null)
            {
                existing.FullName = message.FullName;
                existing.Email = message.Email;
                existing.PhoneNo = message.PhoneNo;
                existing.Role = message.Role;
                existing.IsActive = message.IsActive;
                existing.EmailConfirmed = message.EmailConfirmed;
                existing.TwoFactorEnabled = message.TwoFactorEnabled;
                existing.LastUpdatedAt = message.Timestamp;

                await _userSummaryRepository.UpdateAsync(existing);
            }
            else if (message.EventType == "Deleted" && existing != null)
            {
                await _userSummaryRepository.DeleteAsync(message.UserId);
            }
        }
    }
}
