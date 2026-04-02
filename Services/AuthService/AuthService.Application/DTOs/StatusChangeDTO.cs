using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class StatusChangeDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
