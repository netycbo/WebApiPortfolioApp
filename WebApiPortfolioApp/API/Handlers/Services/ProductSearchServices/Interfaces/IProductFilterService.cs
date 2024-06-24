using WebApiPortfolioApp.API.DTOs.Helpers;

namespace WebApiPortfolioApp.API.Handlers.Services.Interfaces
{
    public interface IProductFilterService
    {
        //Task<List<RawJsonDto>> FilterProducts(List<RawJsonDto> products,  string shopName);
        List<RawJsonDto> FilterNullValues(List<RawJsonDto> products);
        RawJsonDto GroupByLowestPrice(List<RawJsonDto> products);
    }
}
