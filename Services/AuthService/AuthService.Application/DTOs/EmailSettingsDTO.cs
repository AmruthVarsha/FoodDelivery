using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.DTOs
{
    public class EmailSettingsDTO
    {
        public string SmtpEmail { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderEmailPassword { get; set; }
    }
}
