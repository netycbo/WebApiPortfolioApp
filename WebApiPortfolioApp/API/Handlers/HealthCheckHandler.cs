using MediatR;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestSharp;
using WebApiPortfolioApp.HealthChecks;

namespace WebApiPortfolioApp.API.Handlers
{
    public class HealthCheckHandler : IRequestHandler<HealthCheckRequest, HealthCheckRespons>
    {
        private readonly ApiHealthCheck _apiHealthCheck;
        private readonly DatabaseHealthCheck _databaseHealthCheck;

        public HealthCheckHandler(ApiHealthCheck apiHealthCheck, DatabaseHealthCheck databaseHealthCheck)
        {
            _apiHealthCheck = apiHealthCheck;
            _databaseHealthCheck = databaseHealthCheck;
        }
        public async  Task<HealthCheckRespons> Handle(HealthCheckRequest request, CancellationToken cancellationToken)
        {
            var apiResult = await _apiHealthCheck.CheckHealthAsync(new HealthCheckContext(), cancellationToken);
            var dbResult = await _databaseHealthCheck.CheckHealthAsync(new HealthCheckContext(), cancellationToken);

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
