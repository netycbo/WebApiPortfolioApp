using MediatR;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Respons;

namespace WebApiPortfolioApp.API.Request
{
    public class ProductSearchRequest : IRequest<RawJsonDtoResponse>
    {
        public string SearchProduct { get; set; }
    }
}
