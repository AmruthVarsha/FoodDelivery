using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using System.Security.Cryptography;

namespace AuthService.Application.Service
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public RefreshToken GenerateRefreshToken(string Ip)
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreateByIp = Ip
            };
        }

        public async Task AddRefreshToken(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.AddAsync(refreshToken);
        }

        public async Task<RefreshToken> GetRefreshToken(string token)
        {
            return await _refreshTokenRepository.GetByTokenAsync(token);
        }

        public async Task UpdateRefreshToken(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.UpdateAsync(refreshToken);
        }

        public async Task RevokeAllTokens(string userId, string ip)
        {
            var tokens = await _refreshTokenRepository.GetByUserIdAsync(userId);
            await _refreshTokenRepository.RevokeAll(tokens, ip);
        }
    }
}
