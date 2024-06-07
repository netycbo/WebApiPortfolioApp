using MediatR;
using WebApiPortfolioApp.API.Respons;

namespace WebApiPortfolioApp.API.Request
{
    public class HealthCheckRequest : IRequest<HealthCheckRespons>
    {
        public string HealthCheckType { get; set; } = string.Empty;
    }
}
