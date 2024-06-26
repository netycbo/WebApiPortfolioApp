namespace WebApiPortfolioApp.API.DTOs.Helpers
{
    public class RawJsonDto 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Vendor { get; set; } 
        public decimal? Current_Price { get; set; }
        public List<Price_History>? Price_History { get; set; }
        public StoreName? Store { get; set; }
    }
}
