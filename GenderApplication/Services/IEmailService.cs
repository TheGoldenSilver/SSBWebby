namespace GenderApplication.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
        Task SendVerificationEmailAsync(string to, string verificationLink);
    }
}
