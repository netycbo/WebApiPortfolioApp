using Microsoft.EntityFrameworkCore;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.Data;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public class AveragePriceComperator(IComparePrices comparePrices, AppDbContext context) : IAveragePriceComparator
    {
        public async Task<bool> IsPriceBelowAverageAsync(string productName)
        {
            var averagePrice = await comparePrices.ComparePricesAsync(productName);
            if (!averagePrice.HasValue)
            {
                return false;
            }
            var lastRecord = await context.SearchHistory
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
