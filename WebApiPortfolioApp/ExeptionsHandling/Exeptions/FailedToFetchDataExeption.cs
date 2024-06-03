namespace WebApiPortfolioApp.ExeptionsHandling.Exeptions
{
    public class FailedToFetchDataExeption : Exception
    {
        public FailedToFetchDataExeption(string message) : base(message)
        {
        }
    }
}
