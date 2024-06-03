using WebApiPortfolioApp.API.Respons;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces
{
    public interface IFetchProductDetails
    {
        Task<RawJsonDtoResponse> FetchProductDetails(bool isJob, CancellationToken cancellationToken);
    }
}
