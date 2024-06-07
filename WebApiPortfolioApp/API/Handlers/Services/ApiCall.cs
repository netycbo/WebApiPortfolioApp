using RestSharp;

namespace WebApiPortfolioApp.API.Handlers.Services
{
    public class ApiCall : IApiCall
    {
        private readonly IRestClient _restClient;
        private readonly string _apiKey;

        public ApiCall(IRestClient restClient, string apiKey)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }
        public RestRequest CreateProductSearchRequest(string searchProduct, int numberOfResults)
        {
            if (string.IsNullOrEmpty(searchProduct))
                throw new ArgumentNullException(nameof(searchProduct));
            if (numberOfResults <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfResults));

            var restRequest = new RestRequest("/https://kassal.app/api/v1/products", Method.Get);
            restRequest.AddParameter("search", searchProduct);
            restRequest.AddParameter("size",numberOfResults );
            restRequest.AddParameter("sort", "price_asc");
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
