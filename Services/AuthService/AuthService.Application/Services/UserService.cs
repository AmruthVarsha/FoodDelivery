using AuthService.Application.DTOs;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IRefreshTokenService _refreshTokenService;

        public UserService(IUserRepository userRepository, IAuthRepository authRepository, IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<ProfileDTO> GetProfileAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var roles = await _userRepository.GetRolesAsync(userId);

            return new ProfileDTO
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNo = user.PhoneNo,
                IsEmailConfirmed = user.EmailConfirmed,
                IsTwoFactorEnabled = user.TwoFactorEnabled,
                Role = roles != null && roles.Any() ? string.Join(",", roles) : string.Empty,
                IsActive = user.IsActive,
                CreatedAt = null // CreatedAt not yet available in the User domain entity
            };
        }

        public async Task UpdateProfileAsync(string userId, UpdateProfileDTO model)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            user.FullName = model.FullName;
            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.EmailConfirmed = false;
            }
            user.PhoneNo = model.PhoneNo;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeactivateAccountAsync(string userId, string ipAddress)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var result = await _authRepository.ChangeAccountStatusAsync(userId, false);
            if (!result.Succeded)
                throw new BadRequestException(result.Error);

            await _refreshTokenService.RevokeAllTokens(userId, ipAddress);
        }

        public async Task ReactivateAccountAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var result = await _authRepository.ChangeAccountStatusAsync(userId, true);
            if (!result.Succeded)
                throw new BadRequestException(result.Error);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(string? role, bool? isActive)
        {
            return await _userRepository.GetUsersByFilterAsync(role, isActive);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found");
            return user;
        }

        public async Task<IList<string>?> GetRolesAsync(string userId)
        {
            return await _userRepository.GetRolesAsync(userId);
        }
    }
}
