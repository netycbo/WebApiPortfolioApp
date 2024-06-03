using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;

namespace WebApiPortfolioApp.API.Handlers
{
    public class ProductSearchHandler : IRequestHandler<ProductSearchRequest, RawJsonDtoResponse>
    {
        private readonly IApiCall _apiCall;
        private readonly IMapper _mapper;
        private readonly IProductFilterService _productFilterService;
        private readonly ISaveProductService _productSaveService;
        private readonly IUserIdService _userIdService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductSearchHandler(IApiCall apiCall, IMapper mapper, IProductFilterService productFilterService, 
            ISaveProductService productSaveService, IHttpContextAccessor httpContextAccessor, IUserIdService userIdService)
        {
            _apiCall = apiCall;
            _mapper = mapper;
            _productFilterService = productFilterService;
            _productSaveService = productSaveService;
            _httpContextAccessor = httpContextAccessor;
            _userIdService = userIdService;
        }
        public async Task<RawJsonDtoResponse> Handle(ProductSearchRequest request, CancellationToken cancellationToken)
        {
            var restRequest = _apiCall.CreateProductSearchRequest(request.SearchProduct);
            var response = await _apiCall.ExecuteRequestAsync(restRequest, cancellationToken);
            Console.WriteLine($"Response Content: {response.Content}");

            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            { 
                throw new FailedToFetchDataExeption("Failed to fetch data");
            }

            try
            {
                var serializer = new JsonSerializer();
                var rawProductResponse = serializer.Deserialize<RawJsonDtoResponse>(new JsonTextReader(new StringReader(response.Content)));

                if (rawProductResponse == null || rawProductResponse.Data == null)
                {
                    throw new CantDeserializeExeption(response.Content);
                }

                var mappedProducts = _mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);

                var filteredProducts = _productFilterService.FilterProducts(mappedProducts, request.SearchProduct);
                    if (filteredProducts.Count == 0) 
                    {
                       throw new NoMatchingFiltredProductsExeption("No matching filtred products");
                    }

                var userId = _userIdService.GetUserId();

                await _productSaveService.SaveProductsAsync(filteredProducts, userId, false);

                return new RawJsonDtoResponse { Data = filteredProducts };
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Json parsing error: {ex.Message}");
                return new RawJsonDtoResponse();
            }
        }
    }
}
