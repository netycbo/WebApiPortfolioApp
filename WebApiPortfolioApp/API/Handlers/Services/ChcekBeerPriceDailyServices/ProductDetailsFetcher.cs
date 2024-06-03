using Newtonsoft.Json;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Respons;
using AutoMapper;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;

namespace WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices
{
    public class ProductDetailsFetcher : IFetchProductDetails
    {
        private readonly IApiCall _apiCall;
        private readonly IProductFilterService _productFilterService;
        private readonly ISaveProductService _productSaveService;
        private readonly IUserIdService _userIdService;
        private readonly IMapper _mapper;

        public ProductDetailsFetcher(IApiCall apiCall, IProductFilterService productFilterService, ISaveProductService productSaveService, IUserIdService userIdService, IMapper mapper)
        {
            _apiCall = apiCall;
            _productFilterService = productFilterService;
            _productSaveService = productSaveService;
            _userIdService = userIdService;
            _mapper = mapper;
        }

        public async Task<RawJsonDtoResponse> FetchProductDetails(bool isJob, CancellationToken cancellationToken)
        {
            try
            {
                var restRequest = _apiCall.CreateProductSearchRequest("Hansa Mango Ipa 0,5l boks");

                var restResponse = await _apiCall.ExecuteRequestAsync(restRequest, cancellationToken);
                if (restResponse.IsSuccessful && !string.IsNullOrEmpty(restResponse.Content))
                {
                    Console.WriteLine($"Response Content: {restResponse.Content}");
                    var serializer = new JsonSerializer();
                    var rawProductResponse = serializer.Deserialize<RawJsonDtoResponse>(new JsonTextReader(new StringReader(restResponse.Content)));
                    if (rawProductResponse == null || rawProductResponse.Data == null)
                    {
                        return new RawJsonDtoResponse();
                    }
                    var mappedProducts = _mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);

                    var filteredProducts = _productFilterService.FilterProducts(mappedProducts, "Hansa Mango Ipa 0,5");

                    await _productSaveService.SaveProductsAsync(filteredProducts, -1, true);

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
