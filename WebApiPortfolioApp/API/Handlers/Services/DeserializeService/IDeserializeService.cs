namespace WebApiPortfolioApp.API.Handlers.Services.DeserializeService
{
    public interface IDeserializeService
    {
        T Deserialize<T>(string json);
    }
}
