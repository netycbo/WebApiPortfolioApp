using MediatR;
using WebApiPortfolioApp.API.Respons;
using System.ComponentModel.DataAnnotations;
using WebApiPortfolioApp.Validation;

namespace WebApiPortfolioApp.API.Request
{
    public class ProductSearchRequest : IRequest<RawJsonDtoResponse>
    {
        [Required]
        [NoSpecialCharactersAttribute]
        public string SearchProduct { get; set; }
        [NoSpecialCharactersAttribute]
        public string Store { get; set; } = string.Empty.ToLower();
        public int NumberOfResults { get; set; } = 100;
    }
}
