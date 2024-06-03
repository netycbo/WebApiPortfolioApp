namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces
{
    public interface IAveragePriceComparator
    {
        Task<bool> IsPriceBelowAverageAsync(string productName);
    }
}
