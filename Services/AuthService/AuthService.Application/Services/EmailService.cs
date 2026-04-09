using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using System.Net;
using System.Net.Mail;

namespace AuthService.Application.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettingsDTO emailSettingsDTO;

        public EmailService(EmailSettingsDTO emailSettingsDTO)
        {
            this.emailSettingsDTO = emailSettingsDTO;
        }

        public async Task SendEmailAsync(string email,string subject,string body)
        {
            using (var smtpClient = new SmtpClient(emailSettingsDTO.SmtpEmail, emailSettingsDTO.SmtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(
                    emailSettingsDTO.SenderEmail,
                    emailSettingsDTO.SenderEmailPassword
                );

                using (var mail = new MailMessage
                {
                    From = new MailAddress(emailSettingsDTO.SenderEmail, "Food Delivery App"),
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
