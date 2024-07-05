using MediatR;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Validation;

namespace WebApiPortfolioApp.API.Request
{
    public class AddProductsToNewsLetterRequest : IRequest<RawJsonDto>
    {
        [NoSpecialCharactersAttribute]
        public string SearchProduct { get; set; }
        [NoSpecialCharactersAttribute]
        public string Shop { get; set; } = string.Empty.ToLower();
    }
}
