using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.DTOs
{
    public class ProfileDTO
    {
        public string UserId { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string PhoneNo { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string Role { get; set; }

    }
}
