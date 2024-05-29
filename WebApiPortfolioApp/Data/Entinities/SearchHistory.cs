using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiPortfolioApp.Data.Entinities
{
    [Table("SearchHistory")]
    public class SearchHistory : AuditableEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public string SearchString { get; set; }
        public DateTime SearchDate { get; set; }
        public string Shop { get; set; }
        public decimal Price { get; set; }
        public bool IsJob { get; set; }
       
    }
}
