using MediatR;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Respons;

namespace WebApiPortfolioApp.API.Request
{
    public class UpdatePriceProduktRequest : IRequest<UpdatePriceProduktRespons>
    {
        public string UserName { get; set; }
    }
}
