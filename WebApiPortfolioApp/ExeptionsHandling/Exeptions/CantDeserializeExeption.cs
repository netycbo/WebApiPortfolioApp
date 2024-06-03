namespace WebApiPortfolioApp.ExeptionsHandling.Exeptions
{
    public class CantDeserializeExeption : Exception
    {
        public CantDeserializeExeption(string message) : base(message)
        {
        }
    }
}
