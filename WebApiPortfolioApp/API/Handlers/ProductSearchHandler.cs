using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;

namespace WebApiPortfolioApp.API.Handlers
{
    public class ProductSearchHandler(IApiCall apiCall, IMapper mapper, IProductFilterService productFilterService,
        ISaveProductService productSaveService, IHttpContextAccessor httpContextAccessor, IUserIdService userIdService,
        IDeserializeService deserializeService, IShopNameValidator shopNameValidator) : IRequestHandler<ProductSearchRequest, RawJsonDtoResponse>
    {
        private readonly IShopNameValidator _shopNameValidator = shopNameValidator;

        public async Task<RawJsonDtoResponse> Handle(ProductSearchRequest request, CancellationToken cancellationToken)
        {
            var restRequest = apiCall.CreateProductSearchRequest(request.SearchProduct, request.NumberOfResults);
            var response = await apiCall.ExecuteRequestAsync(restRequest, cancellationToken);

            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                throw new FailedToFetchDataExeption("Failed to fetch data");
            }

            var rawProductResponse = JsonConvert.DeserializeObject<RawJsonDtoResponse>(response.Content);

            if (rawProductResponse == null || rawProductResponse.Data == null)
            {
                throw new CantDeserializeExeption(response.Content);
            }

            var mappedProducts = mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);
            var filterNullValues = productFilterService.FilterNullValues(mappedProducts);

            if (filterNullValues.Count == 0)
            {
                throw new NoMatchingFiltredProductsExeption("No matching filtered products");
            }
            List<RawJsonDto> filteredByStoreName = filterNullValues;
            if (!string.IsNullOrEmpty(request.Store))
            {
                var shopNameValidatorTask = _shopNameValidator.ValidateShopName(request.Store);
                var shopNameValidator = await shopNameValidatorTask;
                filteredByStoreName = productFilterService.FilterByStoreName(filterNullValues, shopNameValidator);
            }
            var outOfStockFilter = productFilterService.OutOfStockFilter(filteredByStoreName);
            if (outOfStockFilter.Count == 0)
            {
                throw new OutOFStockExeption("Last date in price history is older than 25 days");
            }

            var groupByLowestPrice = productFilterService.GroupByLowestPrice(outOfStockFilter);
            var userId = userIdService.GetUserId();
            try
            {
                await productSaveService.SaveProductsAsync<List<RawJsonDto>>(groupByLowestPrice, userId, false);
            }
            catch (FailedToSaveExeption)
            {
                throw new FailedToSaveExeption($"Error occurred while saving products");
                throw;
            }
            var groupByLowestPriceList = groupByLowestPrice.ToList();
            return new RawJsonDtoResponse { Data = groupByLowestPriceList };
            
        }
    }
}
