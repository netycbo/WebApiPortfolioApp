using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestSharp;
using WebApiPortfolioApp.API.Handlers.Services;

namespace WebApiPortfolioApp.HealthChecks
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly IApiCall _apiCall;
        public ApiHealthCheck(IApiCall apiCall)
        {
          _apiCall = apiCall;
        }
        public  async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = _apiCall.CreateProductSearchRequest("test", 1);
                var response = await _apiCall.ExecuteRequestAsync(request, cancellationToken);
                if (response.IsSuccessful)
                    return HealthCheckResult.Healthy("API is responsive.");
                else
                    return HealthCheckResult.Unhealthy("API is not responsive.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Exception during API check: {ex.Message}");
            }
        }
    }
}
