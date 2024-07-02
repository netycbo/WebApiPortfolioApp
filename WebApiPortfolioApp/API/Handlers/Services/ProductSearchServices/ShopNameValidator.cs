using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class ShopNameValidator : IShopNameValidator
    {
        private readonly List<string> _names;

        public ShopNameValidator(ShopNameList shopNameList)
        {
            _names = shopNameList.Names;
        }
        public Task<string> ValidateShopName(string shopName)
        {
            if (string.IsNullOrWhiteSpace(shopName))
            {
                return Task.FromResult("No matching name found");
            }

            var cleanedShopName = CleanShopName(shopName);

            var matchingName = _names.FirstOrDefault(name =>
                string.Equals(CleanShopName(name), cleanedShopName, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(matchingName ?? "No matching name found");
        }

        private string CleanShopName(string shopName)
        {
            return shopName.Replace(" ", "").Replace("-", "");
        }
    }
}
