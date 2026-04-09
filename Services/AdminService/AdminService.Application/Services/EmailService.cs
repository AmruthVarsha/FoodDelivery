using AdminService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace AdminService.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["Email:SmtpEmail"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            _senderEmail = configuration["Email:SenderEmail"] ?? "";
            _senderPassword = configuration["Email:SenderEmailPassword"] ?? "";
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);

                using (var mail = new MailMessage
                {
                    From = new MailAddress(_senderEmail, "Food Delivery App"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    mail.To.Add(email);
                    await smtpClient.SendMailAsync(mail);
                }
            }
        }
    }
}
