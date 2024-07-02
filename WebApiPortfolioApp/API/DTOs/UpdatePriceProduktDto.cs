using WebApiPortfolioApp.Data.Entinities;


namespace WebApiPortfolioApp.API.DTOs
{
    public class UpdatePriceProduktDto
    {
        public decimal? Price { get; set; }
        public string ProductName { get; set; }
        public StoreName Store { get; set; }
    }
}
