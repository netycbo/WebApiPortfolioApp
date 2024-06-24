using System.Text.Json.Serialization;

namespace WebApiPortfolioApp.API.DTOs.Helpers
{
    public class StoreName
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
