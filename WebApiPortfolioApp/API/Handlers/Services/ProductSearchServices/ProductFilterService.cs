using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class ProductFilterService : IProductFilterService
    {
        public List<RawJsonDto> FilterNullValues(List<RawJsonDto> products)
        {
            return products
                .Where(d => d.Name != null && d.Store != null && d.Store.Name != null &&
                            d.Current_Price != null && d.Current_Price != 0 && d.Vendor != null)
                .ToList();
        }

        public List<RawJsonDto> GroupByLowestPrice(List<RawJsonDto> filteredData)
        {
            var productWithLowestPrices = filteredData
        .OrderBy(d => d.Current_Price)
        .Take(20)
        .ToList();

            return productWithLowestPrices;
        }
        public List<RawJsonDto> OutOfStockFilter(List<RawJsonDto> filteredData)
        {
            var outOfStockFilter = filteredData
         .Where(d => d.Price_History.FirstOrDefault().Date > DateTime.Now.AddDays(-25))
         .GroupBy(d => d.Name)
         .Select(g => g.OrderBy(d => d.Current_Price).Last())
         .Take(20)
         .ToList();
            return outOfStockFilter;
        }
        public List<RawJsonDto> FilterByStoreName(List<RawJsonDto> filteredData, string store)
        {
            var filteredByStoreName = filteredData
               .Where(d => d.Store.Name == store)
                .ToList();

            return filteredByStoreName;
        }
    }
}
