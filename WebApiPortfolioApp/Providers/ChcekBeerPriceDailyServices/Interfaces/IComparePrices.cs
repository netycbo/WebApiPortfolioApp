namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces
{
    public interface IComparePrices
    {
        Task<decimal?> ComparePricesAsync(string productName);
    }
}
