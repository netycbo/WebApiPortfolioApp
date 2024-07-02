using WebApiPortfolioApp.API.DTOs.Helpers;

namespace WebApiPortfolioApp.API.Handlers.Services.Interfaces
{
    public interface IProductFilterService
    {
        List<RawJsonDto> FilterNullValues(List<RawJsonDto> products);
        List<RawJsonDto> GroupByLowestPrice(List<RawJsonDto> products);
        List<RawJsonDto> OutOfStockFilter(List<RawJsonDto> filteredData);
        List<RawJsonDto> FilterByStoreName(List<RawJsonDto> filteredData, string store);
    }
}
