using WebApiPortfolioApp.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace WebApiPortfolioApp.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _appContex;
        public DatabaseHealthCheck(AppDbContext appContext)
        {
            _appContex = appContext;
        }

        public async  Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _appContex.Database.CanConnectAsync(cancellationToken);
                return HealthCheckResult.Healthy("Database connection is OK.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Failed to connect to the database.", ex);
            }
        }
    }
}
