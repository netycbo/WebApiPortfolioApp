using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace WebApiPortfolioApp.Services.SendEmail
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }
        public async Task SendEmailAsync(EmailRequest mailRequest)
        {
            if (string.IsNullOrWhiteSpace(_emailSettings.Email))
                throw new ArgumentException("Sender email configuration is missing.", nameof(_emailSettings.Email));

            if (string.IsNullOrWhiteSpace(mailRequest.ToEmail))
                throw new ArgumentException("Recipient email is not provided.", nameof(mailRequest.ToEmail));
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
        }
    }
}
