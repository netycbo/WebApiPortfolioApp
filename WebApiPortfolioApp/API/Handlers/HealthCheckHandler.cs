using MediatR;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestSharp;
using WebApiPortfolioApp.HealthChecks;

namespace WebApiPortfolioApp.API.Handlers
{
    public class HealthCheckHandler(ApiHealthCheck apiHealthCheck, DatabaseHealthCheck databaseHealthCheck) : IRequestHandler<HealthCheckRequest, HealthCheckRespons>
    {
        public async  Task<HealthCheckRespons> Handle(HealthCheckRequest request, CancellationToken cancellationToken)
        {
            var apiResult = await apiHealthCheck.CheckHealthAsync(new HealthCheckContext(), cancellationToken);
            var dbResult = await databaseHealthCheck.CheckHealthAsync(new HealthCheckContext(), cancellationToken);

            var response = new HealthCheckRespons
            {
                ApiHealthStatus = apiResult.Status.ToString(),
                DatabaseHealthStatus = dbResult.Status.ToString(),
                ApiDescription = apiResult.Description,
                DatabaseDescription = dbResult.Description
            };

            return response;
        }
    }
}
