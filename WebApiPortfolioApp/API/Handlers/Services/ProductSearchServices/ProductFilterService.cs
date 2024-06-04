using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class ProductFilterService : IProductFilterService
    {
        private readonly IShopNameValidator _validator;
        public ProductFilterService(IShopNameValidator validator)
        {
            _validator = validator;
        }
        public async Task<List<RawJsonDto>> FilterProducts(List<RawJsonDto> products, string shopName)
        {
            if (!string.IsNullOrEmpty(shopName))
                {
                   await _validator.ValidateShopName(shopName);
                   products = products.Where(product => product.Store.Name == shopName).ToList();
            }
            return products.Select(product =>
            {
                
                var lowestPrice = product.Price_History.OrderBy(ph => ph.Price).FirstOrDefault();
                //product.Price_History = new List<Price_History> { lowestPrice };
                return product;
            }).ToList();
        }
    }
}
