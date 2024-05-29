namespace WebApiPortfolioApp.Data.Entinities.Identity
{
    public class UserServices
    {
        public bool IsUserEmailConfirmed(ApplicationUser applicationUser)
         => applicationUser.EmailConfirmed ? true : false;
    }
}
