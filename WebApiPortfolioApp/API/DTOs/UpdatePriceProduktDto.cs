using System.Security.Cryptography.X509Certificates;
using WebApiPortfolioApp.API.DTOs.Helpers;
using StoreName = WebApiPortfolioApp.API.DTOs.Helpers.StoreName;

namespace WebApiPortfolioApp.API.DTOs
{
    public class UpdatePriceProduktDto
    {
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public StoreName Store { get; set; }
    }
}
