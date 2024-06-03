using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System.Security.Claims;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Services.SendEmail;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public class PriceCheckJob : IJob
    {
        private readonly IApiCall _apiCall;
        private readonly IMapper _mapper;
        private readonly IComparePrices _comparePrices;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ISaveProductService _productSaveService;
        private readonly IUserIdService _userIdService;
        private readonly IProductFilterService _productFilterService;
		private readonly ViewRender _viewRenderer;

		public PriceCheckJob(IApiCall apiCall, IMapper mapper, IComparePrices comparePrices,  
                            AppDbContext context, IProductFilterService productFilterService, IEmailService emailService,
                            IUserIdService userIdService, ISaveProductService productSaveService, ViewRender viewRenderer)
        {
            _apiCall = apiCall;
            _mapper = mapper;
            _comparePrices = comparePrices;
            _context = context;
            _emailService = emailService;
            _productSaveService = productSaveService;
            _viewRenderer = viewRenderer;
            _productFilterService = productFilterService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
			var emailContent = await _viewRenderer.RenderToStringAsync("SendEmail/NewsLetter/PriceBelowAvarage", null);
			var isJob = context.JobDetail.JobDataMap.GetBoolean("IsJob"); 
            var product = await FetchProductDetails(isJob, context.CancellationToken);
            var productAvgPrice = await _comparePrices.ComparePricesAsync(product.Data[1].Name);
            var isPriceBelowAverage = await CompareAvgPrices(product.Data[1].Name);          
            var subscribedEmails = await GetSubscribedUserEmailsAsync();
                foreach (var email in subscribedEmails)
                {
                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = email,
                        Subject = isPriceBelowAverage ? "The price of beer just dropped" : "The average price of beer has not changed",
                        Body = isPriceBelowAverage ? emailContent : $"The average beer price is: {productAvgPrice}"
                    });
                }
        }
        private async Task<RawJsonDtoResponse> FetchProductDetails(bool isJob, CancellationToken cancellationToken)
        {
            try
            {
                var restRequest = _apiCall.CreateProductSearchRequest("Hansa Mango Ipa 0,5");

                var restResponse = await _apiCall.ExecuteRequestAsync(restRequest, cancellationToken);
                if (restResponse.IsSuccessful && !string.IsNullOrEmpty(restResponse.Content))
                {
                    Console.WriteLine($"Response Content: {restResponse.Content}");
                    var serializer = new JsonSerializer();
                    var rawProductResponse = serializer.Deserialize<RawJsonDtoResponse>(new JsonTextReader(new StringReader(restResponse.Content)));
                    if (rawProductResponse == null || rawProductResponse.Data == null)
                    {
                        return new RawJsonDtoResponse();
                    }
                    var mappedProducts = _mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);

                    var filteredProducts = _productFilterService.FilterProducts(mappedProducts, "Hansa Mango Ipa 0,5");

                    await _productSaveService.SaveProductsAsync(filteredProducts, -1, true );

                    return new RawJsonDtoResponse { Data = filteredProducts };
                }
                else
                {
                    Console.WriteLine($"Failed to fetch data: {restResponse.ErrorMessage}");
                    return new RawJsonDtoResponse();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FetchProductDetails: {ex.Message}");
                return new RawJsonDtoResponse();
            }
        }

        private async  Task<bool> CompareAvgPrices(string productName)
        {
            var averagePrice = await _comparePrices.ComparePricesAsync(productName);
            if (!averagePrice.HasValue)
            {
                return false; 
            }
            var lastRecord = await _context.SearchHistory
                .Where(sh => sh.SearchString == productName)
                .OrderByDescending(sh => sh.Id)
                .FirstOrDefaultAsync();

            if (lastRecord == null)
            {
                return false; 
            }

            return lastRecord.Price < averagePrice.Value;
        }
        private async Task<List<string>> GetSubscribedUserEmailsAsync()
        {
            return await _context.Users
                                 .Where(user => user.IsSubscribedToLowBeerPriceAletr)
                                 .Select(user => user.Email!)
                                 .ToListAsync();
        }
    }
}
