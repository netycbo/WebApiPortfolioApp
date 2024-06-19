using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using RestSharp;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
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
        private readonly IDeserializeService _deserializeService;

        public ProductSearchHandler(IApiCall apiCall, IMapper mapper, IProductFilterService productFilterService,
            ISaveProductService productSaveService, IHttpContextAccessor httpContextAccessor, IUserIdService userIdService,
            IDeserializeService deserializeService)
        {
            _apiCall = apiCall;
            _mapper = mapper;
            _productFilterService = productFilterService;
            _productSaveService = productSaveService;
            _httpContextAccessor = httpContextAccessor;
            _userIdService = userIdService;
            _deserializeService = deserializeService;
        }

        public async Task<RawJsonDtoResponse> Handle(ProductSearchRequest request, CancellationToken cancellationToken)
        {
            var initialNumberOfResults = request.NumberOfResults;
            var maxRetries = 5;
            var retries = 0;
            List<JToken> filteredData = new List<JToken>();

            while (retries < maxRetries)
            {
                var restRequest = _apiCall.CreateProductSearchRequest(request.SearchProduct, initialNumberOfResults);
                var response = await _apiCall.ExecuteRequestAsync(restRequest, cancellationToken);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    throw new FailedToFetchDataExeption("Failed to fetch data");
                }

                try
                {
                    var jsonResponse = JToken.Parse(response.Content);

                    filteredData = jsonResponse["data"]
                        .Where(item => item["current_price"] != null && item["current_price"].Type != JTokenType.Null)
                        .ToList();

                    if (filteredData.Any())
                    {
                        break;
                    }

                    retries++;
                    initialNumberOfResults += 20; // Zwiększ liczbę wyników o 10
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Json parsing error: {ex.Message}");
                    return new RawJsonDtoResponse();
                }
            }

            if (!filteredData.Any())
            {
                throw new NoMatchingFiltredProductsExeption("No matching filtered products");
            }

            var filteredJsonResponse = new JObject
            {
                ["data"] = new JArray(filteredData)
            };

            var rawProductResponse = JsonConvert.DeserializeObject<RawJsonDtoResponse>(filteredJsonResponse.ToString());

            if (rawProductResponse == null || rawProductResponse.Data == null)
            {
                throw new CantDeserializeExeption(filteredJsonResponse.ToString());
            }

            var mappedProducts = _mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);

            var filteredProducts = await _productFilterService.FilterProducts(mappedProducts, request.Shop);

            if (filteredProducts.Count == 0)
            {
                throw new NoMatchingFiltredProductsExeption("No matching filtered products");
            }

            var userId = _userIdService.GetUserId();

            //await _productSaveService.SaveProductsAsync(filteredProducts, userId, false);

            return new RawJsonDtoResponse { Data = filteredProducts };
        }
    }
}

