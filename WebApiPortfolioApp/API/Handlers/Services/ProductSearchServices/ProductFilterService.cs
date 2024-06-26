using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class ProductFilterService : IProductFilterService
    {
        public List<RawJsonDto> FilterNullValues (List<RawJsonDto> products)
        {
            return products
                .Where(d => d.Name != null && d.Store != null && d.Store.Name != null && 
                            d.Current_Price != null && d.Current_Price != 0 && d.Vendor != null)
                .ToList();
        }

        public RawJsonDto GroupByLowestPrice(List<RawJsonDto> filteredData)
        {
            var productWithLowestPrice = filteredData
         .GroupBy(d => d.Name)
         .Select(g => g.OrderBy(d => d.Current_Price).Last())
         .LastOrDefault();

            return productWithLowestPrice;
        }
    }
}
