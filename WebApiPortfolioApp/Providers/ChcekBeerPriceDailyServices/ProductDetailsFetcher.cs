using Newtonsoft.Json;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Respons;
using AutoMapper;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public class ProductDetailsFetcher : IFetchProductDetails
    {
        private readonly IApiCall _apiCall;
        private readonly IProductFilterService _productFilterService;
        private readonly ISaveProductService _productSaveService;
        private readonly IMapper _mapper;
        private readonly IDeserializeService _deserializeService;

        public ProductDetailsFetcher(IApiCall apiCall, IProductFilterService productFilterService, ISaveProductService productSaveService,
             IMapper mapper, IDeserializeService deserializeService)
        {
            _apiCall = apiCall;
            _productFilterService = productFilterService;
            _productSaveService = productSaveService;
            _mapper = mapper;
            _deserializeService = deserializeService;
        }

        public async Task<RawJsonDtoResponse> FetchProductDetails(bool isJob, CancellationToken cancellationToken)
        {
            try
            {
                var restRequest = _apiCall.CreateProductSearchRequest("Hansa Mango Ipa 0,5l boks", numberOfResults: 10);

                var restResponse = await _apiCall.ExecuteRequestAsync(restRequest, cancellationToken);
                if (restResponse.IsSuccessful && !string.IsNullOrEmpty(restResponse.Content))
                {
                    Console.WriteLine($"Response Content: {restResponse.Content}");
                    
                    var rawProductResponse = _deserializeService.Deserialize<RawJsonDtoResponse>(restResponse.Content);
                    if (rawProductResponse == null || rawProductResponse.Data == null)
                    {
                        return new RawJsonDtoResponse();
                    }
                    var mappedProducts = _mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);

                    var filteredProducts =  _productFilterService.FilterNullValues(mappedProducts);
                    var lowestPrice = _productFilterService.GroupByLowestPrice(filteredProducts).ToList();

                    await _productSaveService.SaveProductsAsync<List<RawJsonDto>>(lowestPrice, "-1", true);

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
