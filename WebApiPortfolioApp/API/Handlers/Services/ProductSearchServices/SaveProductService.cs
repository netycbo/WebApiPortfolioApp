using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Data.Entinities;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class SaveProductService : ISaveProductService
    {
        private readonly AppDbContext _context;

        public SaveProductService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
        }
        public async Task SaveProductsAsync<T>(List<RawJsonDto> products, string userId, bool isJob) where T : class
        {
            var searchHistory = products.Select(product =>
            {
                var now = DateTime.Now;
                var priceHistory = product.Price_History.FirstOrDefault();
                return new SearchHistory
                {
                    UserId = isJob ? "-1" : userId,
                    IsJob = isJob,
                    SearchString = product.Name,
                    SearchDate = DateTime.UtcNow,
                    Store = product?.Store.Name ?? null,
                    Price = priceHistory?.Price ?? 0,
                    Created = now,
                };
            }).ToList();

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
