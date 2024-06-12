using Newtonsoft.Json;

namespace WebApiPortfolioApp.API.Handlers.Services.DeserializeService
{
    public class DeserializeService : IDeserializeService
    {
        public T Deserialize<T>(string json)
        {
            var serializer = new JsonSerializer();
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}
