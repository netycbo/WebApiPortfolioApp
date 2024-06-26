using WebApiPortfolioApp.API.DTOs.Helpers;

namespace WebApiPortfolioApp.API.DTOs
{
    public class TemporaryProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public StoreName Store { get; set; }
    }
}
