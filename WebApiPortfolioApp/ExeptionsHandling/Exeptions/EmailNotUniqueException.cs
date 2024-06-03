namespace WebApiPortfolioApp.ExeptionsHandling.Exeptions
{
    public class EmailNotUniqueException : Exception
    {
        public EmailNotUniqueException(string message) : base(message)
        {
        }
    }
}
