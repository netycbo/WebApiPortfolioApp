using System.Security.Claims;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Data.Entinities;
using Microsoft.Extensions.Logging;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices;
using WebApiPortfolioApp.API.DTOs.Helpers;
using Newtonsoft.Json;

namespace WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices
{
    public class SaveToProductSubscriptionService : ISaveToProductSubscriptionService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SaveToProductSubscriptionService> _logger;
        private readonly IUserIdService _userIdService;

        public SaveToProductSubscriptionService(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<SaveToProductSubscriptionService> logger, IUserIdService userIdService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _userIdService = userIdService;
        }

        public async Task SaveToProductSubscriptionAsync(List<AddProductsToNewsLetterDto> products, string userId, string userName)
        {
            try
            {
                var productSubscriptions = products.Select(products => new ProductSubscription
                {
                    ProductName = products.ProductName ?? string.Empty,
                    Created = DateTime.UtcNow,
                    UserId = userId,
                    UserName = userName,
                    Shop = products.Store,
                    Price = products.Price

                }).ToList();
                Console.WriteLine($"ProductSubscription: {JsonConvert.SerializeObject(productSubscriptions)}");

                _context.ProductSubscriptions.AddRange(productSubscriptions);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving product subscriptions.");
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);

                throw;
            }
        }

        
    }
}
