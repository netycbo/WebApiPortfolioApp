using System.Text.Json.Serialization;

namespace WebApiPortfolioApp.API.DTOs.Helpers
{
    public class PriceHistory
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }
}
