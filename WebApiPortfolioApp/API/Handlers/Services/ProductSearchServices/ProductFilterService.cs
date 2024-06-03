using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class ProductFilterService : IProductFilterService
    {
        public List<RawJsonDto> FilterProducts(List<RawJsonDto> products, string filterName)
        {
            var filteredProducts = products.Where(product => product.Name.Equals(filterName, StringComparison.OrdinalIgnoreCase)).ToList();
            return filteredProducts.Select(product =>
            {
                var lowestPrice = product.Price_History.OrderBy(ph => ph.Price).FirstOrDefault();
                product.Price_History = new List<Price_History> { lowestPrice };
                return product;
            }).ToList();
        }
    }

}
