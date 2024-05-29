using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;

namespace WebApiPortfolioApp.API.Handlers.Services.Interfaces
{
    public interface ISaveProductService
    {
        Task SaveProductsAsync(List<RawJsonDto> products, int userId, bool isJob);
    }
}
