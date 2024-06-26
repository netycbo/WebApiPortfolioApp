using MediatR;
using WebApiPortfolioApp.API.Respons;
using System.ComponentModel.DataAnnotations;

namespace WebApiPortfolioApp.API.Request
{
    public class ProductSearchRequest : IRequest<RawJsonDtoResponse>
    {
        [Required]
        public string SearchProduct { get; set; }
        public string Shop { get; set; } = string.Empty.ToLower();
        public int NumberOfResults { get; set; } = 50;
    }
}
