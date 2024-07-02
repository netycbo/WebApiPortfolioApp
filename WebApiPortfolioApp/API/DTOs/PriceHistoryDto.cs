namespace WebApiPortfolioApp.API.DTOs
{
    public class PriceHistoryDto
    {
        public string ProductName { get; set; }
        public decimal PriceMax { get; set; }
        public DateTime DateOfMaxPrice { get; set; }
        public string StoreWithMaxPrice { get; set; }
        public decimal PriceMin { get; set; }
        public DateTime DateOfMinPrice { get; set; }
        public string StoreWithMinPrice { get; set; }
        public decimal PriceDifference { get; set; }
        public decimal PriceAverage { get; set; }
    }
}

