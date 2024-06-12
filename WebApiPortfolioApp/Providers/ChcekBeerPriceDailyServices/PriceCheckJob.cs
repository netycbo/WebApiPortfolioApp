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
using WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices;
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
		private readonly ViewRender _viewRenderer;
        private readonly IFetchProductDetails _productDetailsFetcher;
        private readonly IAveragePriceComparator _averagePriceComparator;
        private readonly IGetEmailService _getEmailService;

        public PriceCheckJob(IApiCall apiCall, IMapper mapper, IComparePrices comparePrices,  
                            AppDbContext context, IEmailService emailService,
                            ISaveProductService productSaveService, ViewRender viewRenderer,
                            IFetchProductDetails productDetailsFetcher, IAveragePriceComparator averagePriceComparator, IGetEmailService getEmailService)
        {
            _apiCall = apiCall;
            _mapper = mapper;
            _comparePrices = comparePrices;
            _context = context;
            _emailService = emailService;
            _productSaveService = productSaveService;
            _viewRenderer = viewRenderer;
            _productDetailsFetcher = productDetailsFetcher;
            _averagePriceComparator = averagePriceComparator;
            _getEmailService = getEmailService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
			var emailContent = await _viewRenderer.RenderToStringAsync("SendEmail/NewsLetter/PriceBelowAvarage", null);
			var isJob = context.JobDetail.JobDataMap.GetBoolean("IsJob"); 
            var product = await _productDetailsFetcher.FetchProductDetails(isJob, context.CancellationToken);
            var productAvgPrice = await _comparePrices.ComparePricesAsync(product.Data[1].Name);
            var isPriceBelowAverage = await _averagePriceComparator.IsPriceBelowAverageAsync(product.Data[1].Name);
            var subscribedEmails = await _getEmailService.GetMailingList();
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
    }
}
