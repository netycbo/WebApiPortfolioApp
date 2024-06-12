using Microsoft.AspNetCore.Identity;

namespace WebApiPortfolioApp.Data.Entinities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsSubscribedToNewsLetter { get; set; }
        public virtual ICollection<ProductSubscription> ProductSubscriptions { get; set; }
    }
}
