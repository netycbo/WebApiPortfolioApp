using WebApiPortfolioApp.API.DTOs.Helpers;

namespace WebApiPortfolioApp.API.Handlers.Services.Interfaces
{
    public interface IProductFilterService
    {
        List<RawJsonDto> FilterNullValues(List<RawJsonDto> products);
        RawJsonDto GroupByLowestPrice(List<RawJsonDto> products);
    }
}
