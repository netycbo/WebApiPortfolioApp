namespace WebApiPortfolioApp.API.DTOs
{
    public class AddProductsToNewsLetterDto
    {
        public string ProductName { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
    }
}
