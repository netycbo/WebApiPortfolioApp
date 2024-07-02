using WebApiPortfolioApp.Data.Entinities;

namespace WebApiPortfolioApp.API.DTOs
{
    public class AddProductsToNewsLetterDto
    {
        public string ProductName { get; set; }
        public string UserName { get; set; }
        public StoreName Store { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal Price { get; set; }
    }
}
