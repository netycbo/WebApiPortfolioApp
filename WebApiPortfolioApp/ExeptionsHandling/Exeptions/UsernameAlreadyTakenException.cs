namespace WebApiPortfolioApp.ExeptionsHandling.Exeptions
{
    public class UsernameAlreadyTakenException : Exception
    {
        public UsernameAlreadyTakenException(string message) : base(message)
        {
        }
    }
}
