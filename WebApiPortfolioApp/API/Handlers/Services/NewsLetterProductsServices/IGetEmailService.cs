namespace WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices
{
    public interface IGetEmailService
    {
        Task<List<string>> GetMailingList();
    }
}
