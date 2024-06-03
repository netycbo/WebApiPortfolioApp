namespace WebApiPortfolioApp.ExeptionsHandling.Exeptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message)
        {
        }
    }
}
