using WebApiPortfolioApp.API.DTOs.Helpers;

namespace WebApiPortfolioApp.API.DTOs
{
    public class AddProductsToNewsLetterDto
    {
        public string ProductName { get; set; }
        public string UserName { get; set; }
        public string Store { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal Price { get; set; }
    }
}
