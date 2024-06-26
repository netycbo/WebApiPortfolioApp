using System.Security.Claims;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Data.Entinities;
using WebApiPortfolioApp.Providers;
using System.Linq;
using WebApiPortfolioApp.API.DTOs;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class SaveProductService : ISaveProductService
    {
        private readonly AppDbContext _context;

        public SaveProductService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
        }
        public async Task SaveProductsAsync<T>(RawJsonDto products, string userId, bool isJob) where T : class
        {
            var priceHistory = products.Price_History?.FirstOrDefault();

            var searchHistory = new SearchHistory
            {
                UserId = isJob ? "-1" : userId,
                IsJob = isJob,
                SearchString = products.Name ?? string.Empty,
                SearchDate = DateTime.UtcNow,
                Shop = products.Store.Name, 
                Price = priceHistory?.Price ?? 0,
                Created = DateTime.UtcNow,
            };

            _context.SearchHistory.AddRange(searchHistory);
            await _context.SaveChangesAsync();
        }

        public async Task SaveTemporaryProductsAsync(List<TemporaryProductsDto> products)
        {
            var temporaryProducts = products.Select(product =>
            {
                return new TemporaryProduct
                {
                    Price = product.Price,
                    Name = product.Name,
                    Store = product.Store.Name
                };
            }).ToList();


            _context.TemporaryProducts.AddRange(temporaryProducts);
            await _context.SaveChangesAsync();
        }
    }
}
