using System;
using System.Collections.Generic;
using System.Text;
using AuthService.Domain.Entities;

namespace AuthService.Application.DTOs
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }
        public RefreshToken? RefreshToken { get; set; }
        public bool RequireTwoFactor { get; set; }
    }
}
