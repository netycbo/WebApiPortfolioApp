using Quartz;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices;

namespace WebApiPortfolioApp.Extensions
{
    public static class QuartzExtensions
    {
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.AddJob<PriceCheckJob>(opts => opts.WithIdentity("PriceCheckJob").StoreDurably());
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            return services;
        }
    }
}
