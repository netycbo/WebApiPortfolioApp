using Microsoft.EntityFrameworkCore;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.Data;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public class AveragePriceComperator : IAveragePriceComparator
    {
        private readonly IComparePrices _comparePrices;
        private readonly AppDbContext _context;

        public AveragePriceComperator(IComparePrices comparePrices, AppDbContext context)
        {
            _comparePrices = comparePrices;
            _context = context;
        }
        public async Task<bool> IsPriceBelowAverageAsync(string productName)
        {
            var averagePrice = await _comparePrices.ComparePricesAsync(productName);
            if (!averagePrice.HasValue)
            {
                return false;
            }
            var lastRecord = await _context.SearchHistory
                .Where(sh => sh.SearchString == productName)
                .OrderByDescending(sh => sh.Id)
                .FirstOrDefaultAsync();

            if (lastRecord == null)
            {
                return false;
            }

            return lastRecord.Price < averagePrice.Value;
        }
    }
}
