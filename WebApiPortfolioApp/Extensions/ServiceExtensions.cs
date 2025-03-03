using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices;

namespace WebApiPortfolioApp.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IProductFilterService, ProductFilterService>();
            services.AddScoped<ISaveProductService, SaveProductService>();
            services.AddScoped<IUserIdService, UserIdService>();
            services.AddScoped<IComparePrices, ComparePrices>();
            services.AddScoped<IFetchProductDetails, ProductDetailsFetcher>();
            services.AddScoped<IAveragePriceComparator, AveragePriceComperator>();
            services.AddScoped<IShopNameValidator, ShopNameValidator>();
            services.AddScoped<IDeserializeService, DeserializeService>();
            services.AddScoped<IUserNameClaimService, UserNameClaimService>();
            services.AddScoped<IGetEmailService, GetEmailService>();
            services.AddScoped<ISaveToProductSubscriptionService, SaveToProductSubscriptionService>();

            return services;
        }
    }
}
