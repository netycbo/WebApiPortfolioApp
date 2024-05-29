using Quartz;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public static class JobScheduler
    {
        public static async Task ScheduleJob(IScheduler scheduler)
        {
            var job = JobBuilder.Create<PriceCheckJob>()
                .WithIdentity("ProductPriceCheck")
                .StoreDurably()
                .UsingJobData("IsJob", true)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("DailyTrigger", "default")
                .StartNow()
                .WithCronSchedule("0 0/2 * * * ?") 
                .ForJob(job)

                .Build();

            await scheduler.AddJob(job, true);
            await scheduler.ScheduleJob(trigger);
        }
    }
}