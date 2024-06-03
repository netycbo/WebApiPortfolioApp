namespace WebApiPortfolioApp.ExeptionsHandling.Exeptions
{
    public class NoMatchingFiltredProductsExeption : Exception
    {
        public NoMatchingFiltredProductsExeption(string message) : base(message)
        {
        }
    }
}
