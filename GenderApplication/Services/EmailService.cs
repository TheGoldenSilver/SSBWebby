using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace GenderApplication.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"],
                _configuration["EmailSettings:SenderEmail"]));
            emailMessage.To.Add(MailboxAddress.Parse(to));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
                bodyBuilder.HtmlBody = body;
            else
                bodyBuilder.TextBody = body;

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:SmtpPort"]),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _configuration["EmailSettings:SmtpUsername"],
                _configuration["EmailSettings:SmtpPassword"]);

            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }

        public async Task SendVerificationEmailAsync(string to, string verificationLink)
        {
            string subject = "Verify Your Email Address";
            string body = $@"
                <h2>Welcome to SSB Webby!</h2>
                <p>Thank you for registering. Please click the button below to verify your email address:</p>
                <p>
                    <a href='{verificationLink}' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>
                        Verify Email
                    </a>
                </p>
                <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                <p>{verificationLink}</p>
                <p>This link will expire in 24 hours.</p>
                <p>If you didn't create an account, you can safely ignore this email.</p>";

            await SendEmailAsync(to, subject, body, isHtml: true);
        }
    }
}
