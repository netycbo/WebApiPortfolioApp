using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;

namespace WebApiPortfolioApp.API.Handlers.Services.Interfaces
{
    public interface ISaveProductService
    {
        Task SaveProductsAsync<T>(List<T> products, string userId, bool isJob) where T : class;
    }
}
