namespace WebApiPortfolioApp.ExeptionsHandling.Exeptions
{
    public class FailedToSaveExeption : Exception
    {
        public FailedToSaveExeption(string message) : base(message)
        {
        }
    }
}
