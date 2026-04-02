using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.DTOs
{
    public class JwtSettingsDTO
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double ExpiryMinutes { get; set; }
    }
}
