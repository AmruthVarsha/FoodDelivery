using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        public RefreshToken GenerateRefreshToken(string Ip);

        public Task AddRefreshToken(RefreshToken refreshToken);

        public Task<RefreshToken> GetRefreshToken(string token);

        public Task UpdateRefreshToken(RefreshToken refreshToken);

        public Task RevokeAllTokens(string userId,string ip);
    }
}
