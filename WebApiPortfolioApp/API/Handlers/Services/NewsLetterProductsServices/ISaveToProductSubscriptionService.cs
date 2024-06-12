using WebApiPortfolioApp.API.DTOs;

namespace WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices
{
    public interface ISaveToProductSubscriptionService
    {
        Task SaveToProductSubscriptionAsync(List<AddProductsToNewsLetterDto> products, string userId, string userName);
    }
}
