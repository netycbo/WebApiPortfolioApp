using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public class ComparePrices(AppDbContext context) : IComparePrices
    {
        public async Task<decimal?> ComparePricesAsync(string productName)
        {
            var averagePrice = await context.SearchHistory
            .Where(sh => sh.SearchString == productName)
            .Select(sh => (decimal?)sh.Price) 
            .AverageAsync();

            return averagePrice;
        }
    }
}
