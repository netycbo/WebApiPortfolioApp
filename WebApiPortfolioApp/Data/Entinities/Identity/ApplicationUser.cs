using Microsoft.AspNetCore.Identity;

namespace WebApiPortfolioApp.Data.Entinities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsSubscribedToLowBeerPriceAletr { get; set; }
    }
}
