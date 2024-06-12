using MediatR;
using WebApiPortfolioApp.API.Respons;

namespace WebApiPortfolioApp.API.Request
{
    public class AddProductsToNewsLetterRequest : IRequest<AddProductsToNewsLetterRespons>
    {
        public string SearchProduct { get; set; }
        public string Shop { get; set; } = string.Empty.ToLower();
    }
}
