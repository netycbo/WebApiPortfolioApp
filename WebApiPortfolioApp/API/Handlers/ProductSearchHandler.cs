using AutoMapper;
using MediatR;
using Newtonsoft.Json;
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
            var restRequest = _apiCall.CreateProductSearchRequest(request.SearchProduct, request.NumberOfResults);
            var response = await _apiCall.ExecuteRequestAsync(restRequest, cancellationToken);

            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                throw new FailedToFetchDataExeption("Failed to fetch data");
            }

            var rawProductResponse = JsonConvert.DeserializeObject<RawJsonDtoResponse>(response.Content);

            if (rawProductResponse == null || rawProductResponse.Data == null)
            {
                throw new CantDeserializeExeption(response.Content);
            }

            var mappedProducts = _mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);
            var filterNullValues = _productFilterService.FilterNullValues(mappedProducts);
            var groupByLowestPrice = _productFilterService.GroupByLowestPrice(filterNullValues);
            //var filteredProducts = await _productFilterService.FilterProducts(groupByLowestPrice, request.Shop);

            //if (filteredProducts != 0)
            //{
            //    throw new NoMatchingFiltredProductsExeption("No matching filtered products");
            //}

            var userId = _userIdService.GetUserId();
            try
            {
                await _productSaveService.SaveProductsAsync<RawJsonDto>(groupByLowestPrice, userId, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while saving products: {ex.Message}");
            }
            return new RawJsonDtoResponse { Data = new List<RawJsonDto> { groupByLowestPrice } };
        }
    }
}

