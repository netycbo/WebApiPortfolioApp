using MediatR;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Respons;

namespace WebApiPortfolioApp.API.Request
{
    public class AddProductsToNewsLetterRequest : IRequest<RawJsonDto>
    {
        public string SearchProduct { get; set; }
        public string Shop { get; set; } = string.Empty.ToLower();
    }
}
