using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class ShopNameValidator : IShopNameValidator
    {
        private readonly List<string> names;
        public ShopNameValidator(List<string> names)
        {
            this.names = names;
        }
        public string ValidateShopName(string shopName)
        {
            var replaceWhitespaces = shopName.Replace(" ", "");
            var matchingNames = names.FirstOrDefault(name =>
                string.Equals(name, replaceWhitespaces, StringComparison.OrdinalIgnoreCase));

            return  matchingNames ?? "No matching name found";

        }
    }
}
