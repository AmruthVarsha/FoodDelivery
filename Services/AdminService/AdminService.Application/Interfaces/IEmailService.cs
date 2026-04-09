namespace AdminService.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
