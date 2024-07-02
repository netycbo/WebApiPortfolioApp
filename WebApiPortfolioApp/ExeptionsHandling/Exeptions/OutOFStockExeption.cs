namespace WebApiPortfolioApp.ExeptionsHandling.Exeptions
{
    public class OutOFStockExeption : Exception
    {
        public OutOFStockExeption(string message) : base(message)
        {
        }
    }
}
