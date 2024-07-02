using Castle.Components.DictionaryAdapter;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiPortfolioApp.Data.Entinities.Identity;

namespace WebApiPortfolioApp.Data.Entinities
{
    [Table("SearchHistory")]
    public class SearchHistory : AuditableEntity
    {
        public int Id { get; set; }
        [MaxLength]
        public string UserId { get; set; } 
        public string SearchString { get; set; }
        public DateTime SearchDate { get; set; }
        public string Store { get; set; }
        public decimal Price { get; set; }
        public bool IsJob { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

    }
}
