namespace WebApiPortfolioApp.ExeptionsHandling.Exeptions
{
    public class CantCreateUserExeption : Exception
    {
        public CantCreateUserExeption(string message) : base(message)
        {
        }
    }
}
