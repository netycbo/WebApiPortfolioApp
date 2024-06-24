using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class ProductFilterService : IProductFilterService
    {
        private readonly IShopNameValidator _validator;

        public ProductFilterService(IShopNameValidator validator)
        {
            _validator = validator;
        }

        //public async Task<List<RawJsonDto>> FilterProducts(List<RawJsonDto> products, string shopName)
        //{
        //    if (!string.IsNullOrEmpty(shopName))
        //    {
        //        var validatedShopName = await _validator.ValidateShopName(shopName);
        //        if (validatedShopName != "No matching Shop name found")
        //        {
        //            products = products.Where(product => product.Store.Name == validatedShopName).ToList();
        //        }
        //    }

        //    var filteredData = FilterNullValues(products);
        //    var groupedData = GroupByLowestPrice(filteredData);

        //    return groupedData;
        //}

        public List<RawJsonDto> FilterNullValues(List<RawJsonDto> products)
        {
            return products
                .Where(d => d.Name != null && d.Store != null && d.Store.Name != null &&
                            d.Current_Price != null && d.Current_Price != 0 && d.Vendor != null)
                .ToList();
        }

        public Task<List<RawJsonDto>> FilterProducts(List<RawJsonDto> products, string shopName)
        {
            throw new NotImplementedException();
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
