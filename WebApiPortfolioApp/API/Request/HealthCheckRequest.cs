using MediatR;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Validation;

namespace WebApiPortfolioApp.API.Request
{
    public class HealthCheckRequest : IRequest<HealthCheckRespons>
    {
        [NoSpecialCharactersAttribute]
        public string HealthCheckType { get; set; } = string.Empty;
    }
}
