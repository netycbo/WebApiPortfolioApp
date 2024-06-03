using WebApiPortfolioApp.API.DTOs.Helpers;

namespace WebApiPortfolioApp.API.Handlers.Services.Interfaces
{
    public interface IProductFilterService
    {
        List<RawJsonDto> FilterProducts(List<RawJsonDto> products, string filterName);
    }
}
