using RestSharp;

namespace WebApiPortfolioApp.API.Handlers.Services
{
    public class ApiCall : IApiCall
    {
        private readonly IRestClient _restClient;
        private readonly string _apiKey;

        public ApiCall(IRestClient restClient, string apiKey)
        {
            _restClient = restClient;
            _apiKey = apiKey;
        }
        public RestRequest CreateProductSearchRequest(string searchProduct)
        {
            var restRequest = new RestRequest("/https://kassal.app/api/v1/products", Method.Get);
            restRequest.AddParameter("search", searchProduct);
            restRequest.AddParameter("size", 10);
            restRequest.AddParameter("sort", "price_desc");
            restRequest.AddHeader("Authorization", $"Bearer {_apiKey}");
            return restRequest;
        }
        public async Task<RestResponse> ExecuteRequestAsync(RestRequest request, CancellationToken cancellationToken)
        {
            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            return response;
        }
    }
}
