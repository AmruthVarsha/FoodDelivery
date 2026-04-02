using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        public Task AddAsync(RefreshToken token);
        public Task<RefreshToken?> GetByTokenAsync(string token);
        public Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId);
        public Task UpdateAsync(RefreshToken token);
        public Task DeleteAsync(RefreshToken token);
        public Task RevokeAll(IEnumerable<RefreshToken> tokens,string Ip);
    }
}
