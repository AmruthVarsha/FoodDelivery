using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Repositories
{
    public class UserSummaryRepository : IUserSummaryRepository
    {
        private readonly AdminDbContext _context;

        public UserSummaryRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserSummary user)
        {
            await _context.UserSummaries.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserSummary>> GetAllAsync()
        {
            return await _context.UserSummaries.ToListAsync();
        }

        public async Task<UserSummary?> GetByUserIdAsync(string userId)
        {
            return await _context.UserSummaries.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<UserSummary?> GetByEmailAsync(string email)
        {
            return await _context.UserSummaries.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateAsync(UserSummary user)
        {
            _context.UserSummaries.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            var user = await _context.UserSummaries.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                _context.UserSummaries.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<UserSummary>> GetByRoleAsync(string role)
        {
            return await _context.UserSummaries.Where(u => u.Role == role).ToListAsync();
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.UserSummaries.CountAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _context.UserSummaries.CountAsync(u => u.IsActive);
        }
    }
}
