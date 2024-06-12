using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiPortfolioApp.Data.Entinities.Identity;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;

namespace WebApiPortfolioApp.Data.Entinities
{
        [Table("ProductSubscriptions")]
        public class ProductSubscription
        {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ProductName { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        }
}

