using RestSharp;


namespace WebApiPortfolioApp.API.Handlers.Services
{
    public interface IApiCall
    {
        RestRequest CreateProductSearchRequest(string searchProduct, int numberOfResults);
        Task<RestResponse> ExecuteRequestAsync(RestRequest request, CancellationToken cancellationToken);
    }
}
