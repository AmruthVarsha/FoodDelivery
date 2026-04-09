using AdminService.Domain.Entities;

namespace AdminService.Domain.Interfaces
{
    public interface IUserSummaryRepository
    {
        Task AddAsync(UserSummary user);
        Task<List<UserSummary>> GetAllAsync();
        Task<UserSummary?> GetByUserIdAsync(string userId);
        Task<UserSummary?> GetByEmailAsync(string email);
        Task UpdateAsync(UserSummary user);
        Task DeleteAsync(string userId);
        Task<List<UserSummary>> GetByRoleAsync(string role);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
    }
}
