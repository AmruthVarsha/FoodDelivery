using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Interfaces
{
    public interface IUserService
    {
        Task<ProfileDTO> GetProfileAsync(string userId);
        Task UpdateProfileAsync(string userId, UpdateProfileDTO model);
        Task DeactivateAccountAsync(string userId, string ipAddress);
        Task ReactivateAccountAsync(string userId);
        Task<IEnumerable<User>> GetAllUsersAsync(string? role, bool? isActive);
        Task<User> GetUserByIdAsync(string id);
        Task<IList<string>?> GetRolesAsync(string userId);
    }
}
