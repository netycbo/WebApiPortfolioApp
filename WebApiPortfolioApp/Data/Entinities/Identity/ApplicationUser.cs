using Microsoft.AspNetCore.Identity;

namespace WebApiPortfolioApp.Data.Entinities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsSubscribedToNewsLetter { get; set; }
        public virtual ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();
        public virtual ICollection<ProductSubscription> ProductSubscriptions { get; set; } = new List<ProductSubscription>();

    }
}

