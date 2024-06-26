using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;

namespace WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices
{
    public interface ISaveToProductSubscriptionService
    {
        Task SaveToProductSubscriptionAsync(AddProductsToNewsLetterDto products, string userId, string userName);
    }
}
