using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string password);
    }
}
