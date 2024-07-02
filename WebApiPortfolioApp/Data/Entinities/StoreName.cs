using System.Text.Json.Serialization;

namespace WebApiPortfolioApp.Data.Entinities
{
    public class StoreName
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
    }
}
