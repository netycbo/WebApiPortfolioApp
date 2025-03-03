using WebApiPortfolioApp.HealthChecks;

namespace WebApiPortfolioApp.Extensions
{
    public static class HealthChecksExtensions
    {
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
        {
            services.AddScoped<DatabaseHealthCheck>();
            services.AddScoped<ApiHealthCheck>();

            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("Database")
                .AddCheck<ApiHealthCheck>("Api");

            return services;
        }
    }
}
