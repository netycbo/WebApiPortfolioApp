using Newtonsoft.Json;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Respons;
using AutoMapper;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public class ProductDetailsFetcher(IApiCall apiCall, IProductFilterService productFilterService, ISaveProductService productSaveService,
         IMapper mapper, IDeserializeService deserializeService) : IFetchProductDetails
    {
        public async Task<RawJsonDtoResponse> FetchProductDetails(bool isJob, CancellationToken cancellationToken)
        {
            try
            {
                var restRequest = apiCall.CreateProductSearchRequest("Hansa Mango Ipa 0,5l boks", numberOfResults: 10);

                var restResponse = await apiCall.ExecuteRequestAsync(restRequest, cancellationToken);
                if (restResponse.IsSuccessful && !string.IsNullOrEmpty(restResponse.Content))
                {
                    Console.WriteLine($"Response Content: {restResponse.Content}");
                    
                    var rawProductResponse = deserializeService.Deserialize<RawJsonDtoResponse>(restResponse.Content);
                    if (rawProductResponse == null || rawProductResponse.Data == null)
                    {
                        return new RawJsonDtoResponse();
                    }
                    var mappedProducts = mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);

                    var filteredProducts =  productFilterService.FilterNullValues(mappedProducts);
                    var lowestPrice = productFilterService.GroupByLowestPrice(filteredProducts).ToList();

                    await productSaveService.SaveProductsAsync<List<RawJsonDto>>(lowestPrice, "-1", true);

                    return new RawJsonDtoResponse { Data = filteredProducts };
                }
                else
                {
                    Console.WriteLine($"Failed to fetch data: {restResponse.ErrorMessage}");
                    return new RawJsonDtoResponse();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FetchProductDetails: {ex.Message}");
                return new RawJsonDtoResponse();
            }
        }

       
    }
}
