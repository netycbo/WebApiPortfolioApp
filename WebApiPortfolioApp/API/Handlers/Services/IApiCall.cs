using RestSharp;


namespace WebApiPortfolioApp.API.Handlers.Services
{
    public interface IApiCall
    {
        RestRequest CreateProductSearchRequest(string searchProduct);
        Task<RestResponse> ExecuteRequestAsync(RestRequest request, CancellationToken cancellationToken);
    }
}
