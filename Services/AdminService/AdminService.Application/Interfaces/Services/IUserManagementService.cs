using AdminService.Application.DTOs.Users;

namespace AdminService.Application.Interfaces.Services
{
    public interface IUserManagementService
    {
        Task<List<UserSummaryDto>> GetAllUsersAsync();
        Task<UserSummaryDto> GetUserByIdAsync(string userId);
        Task<UserSummaryDto> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(string userId, UpdateUserDto dto);
        Task DeleteUserAsync(string userId);
        Task<List<UserSummaryDto>> GetUsersByRoleAsync(string role);
    }
}
