using AdminService.Application.DTOs.Users;
using AdminService.Application.Exceptions;
using AdminService.Application.Interfaces.Services;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Messaging.Publishers;

namespace AdminService.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserSummaryRepository _userSummaryRepository;
        private readonly UserUpdatePublisher _userUpdatePublisher;

        public UserManagementService(
            IUserSummaryRepository userSummaryRepository,
            UserUpdatePublisher userUpdatePublisher)
        {
            _userSummaryRepository = userSummaryRepository;
            _userUpdatePublisher = userUpdatePublisher;
        }

        public async Task<List<UserSummaryDto>> GetAllUsersAsync()
        {
            var users = await _userSummaryRepository.GetAllAsync();
            return users.Select(u => MapToDto(u)).ToList();
        }

        public async Task<UserSummaryDto> GetUserByIdAsync(string userId)
        {
            var user = await _userSummaryRepository.GetByUserIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            return MapToDto(user);
        }

        public async Task<UserSummaryDto> GetUserByEmailAsync(string email)
        {
            var user = await _userSummaryRepository.GetByEmailAsync(email);
            if (user == null)
                throw new NotFoundException($"User with email {email} not found");

            return MapToDto(user);
        }

        public async Task UpdateUserAsync(string userId, UpdateUserDto dto)
        {
            var user = await _userSummaryRepository.GetByUserIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            user.FullName = dto.FullName;
            user.PhoneNo = dto.PhoneNo;
            user.IsActive = dto.IsActive;
            user.LastUpdatedAt = DateTime.UtcNow;

            await _userSummaryRepository.UpdateAsync(user);

            await _userUpdatePublisher.PublishUserUpdate(
                user.UserId, user.FullName, user.Email, user.PhoneNo, user.Role,
                user.IsActive, user.EmailConfirmed, user.TwoFactorEnabled, "Updated");
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userSummaryRepository.GetByUserIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            await _userSummaryRepository.DeleteAsync(userId);

            await _userUpdatePublisher.PublishUserUpdate(
                user.UserId, user.FullName, user.Email, user.PhoneNo, user.Role,
                false, user.EmailConfirmed, user.TwoFactorEnabled, "Deleted");
        }

        public async Task<List<UserSummaryDto>> GetUsersByRoleAsync(string role)
        {
            var users = await _userSummaryRepository.GetByRoleAsync(role);
            return users.Select(u => MapToDto(u)).ToList();
        }

        private static UserSummaryDto MapToDto(Domain.Entities.UserSummary user)
        {
            return new UserSummaryDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNo = user.PhoneNo,
                Role = user.Role,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                CreatedAt = user.CreatedAt,
                LastUpdatedAt = user.LastUpdatedAt
            };
        }
    }
}
