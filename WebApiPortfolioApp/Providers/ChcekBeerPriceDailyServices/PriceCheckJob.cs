using AutoMapper;
using Quartz;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Services.SendEmail;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public class PriceCheckJob(IApiCall apiCall, IMapper mapper, IComparePrices comparePrices,
                        AppDbContext context, IEmailService emailService,
                        ISaveProductService productSaveService, ViewRender viewRenderer,
                        IFetchProductDetails productDetailsFetcher, IAveragePriceComparator averagePriceComparator, IGetEmailService getEmailService) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
			var emailContent = await viewRenderer.RenderToStringAsync("SendEmail/NewsLetter/PriceBelowAvarage", null);
			var isJob = context.JobDetail.JobDataMap.GetBoolean("IsJob"); 
            var product = await productDetailsFetcher.FetchProductDetails(isJob, context.CancellationToken);
            var productAvgPrice = await comparePrices.ComparePricesAsync(product.Data[1].Name);
            var isPriceBelowAverage = await averagePriceComparator.IsPriceBelowAverageAsync(product.Data[1].Name);
            var subscribedEmails = await getEmailService.GetMailingList();
                foreach (var email in subscribedEmails)
                {
                    await emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = email,
                        Subject = isPriceBelowAverage ? "The price of beer just dropped" : "The average price of beer has not changed",
                        Body = isPriceBelowAverage ? emailContent : $"The average beer price is: {productAvgPrice}"
                    });
                }
        }
    }
}
