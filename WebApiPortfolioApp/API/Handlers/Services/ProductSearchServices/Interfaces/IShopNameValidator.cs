namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces
{
    public interface IShopNameValidator
    {
        Task<string> ValidateShopName(string shopName);
    }
}
