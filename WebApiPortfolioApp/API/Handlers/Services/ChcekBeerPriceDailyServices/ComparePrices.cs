using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public class ComparePrices : IComparePrices
    {
        private readonly AppDbContext _context;

        public ComparePrices(AppDbContext context)
        {
            _context = context;
        }
        public async Task<decimal?> ComparePricesAsync(string productName)
        {
            var averagePrice = await _context.SearchHistory
            .Where(sh => sh.SearchString == productName)
            .Select(sh => (decimal?)sh.Price) 
            .AverageAsync();

            return averagePrice;
        }
    }
}
