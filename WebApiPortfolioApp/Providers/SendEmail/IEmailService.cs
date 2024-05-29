namespace WebApiPortfolioApp.Services.SendEmail
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest mailRequest);
    }
}
