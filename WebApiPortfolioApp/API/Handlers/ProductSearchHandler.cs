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
    public class ProductSearchHandler(
        IApiCall apiCall,
        IMapper mapper,
        IProductFilterService productFilterService,
        ISaveProductService productSaveService,
        IUserIdService userIdService,
        IDeserializeService deserializeService,
        IShopNameValidator shopNameValidator) : IRequestHandler<ProductSearchRequest, RawJsonDtoResponse>
    {
        private readonly IShopNameValidator _shopNameValidator = shopNameValidator;

        private const int MaxRetries = 100;

        public async Task<RawJsonDtoResponse> Handle(ProductSearchRequest request, CancellationToken cancellationToken)
        {
            var filterNullValues = new List<RawJsonDto>();
            int attempts = 0;

            while (attempts < MaxRetries)
            {
                attempts++;
                var restRequest = apiCall.CreateProductSearchRequest(request.SearchProduct, request.NumberOfResults);
                var response = await apiCall.ExecuteRequestAsync(restRequest, cancellationToken);

                if (!response.IsSuccessful && string.IsNullOrEmpty(response.Content))
                {
                    throw new FailedToFetchDataExeption("Failed to fetch data");
                }

                var rawProductResponse = deserializeService.Deserialize<RawJsonDtoResponse>(response.Content);

                if (rawProductResponse == null || rawProductResponse.Data == null)
                {
                    throw new CantDeserializeExeption(response.Content);
                }

                filterNullValues = productFilterService.FilterNullValues(mapper.Map<List<RawJsonDto>>(rawProductResponse.Data));

                if (filterNullValues.Count != 0)
                {
                    break;
                }
                else if (request.NumberOfResults < 5 && attempts >= MaxRetries)
                {
                    break;
                }
                else if (attempts >= MaxRetries)
                {
                    throw new NoMatchingFiltredProductsExeption("Too many null values in data");
                }
            }

            List<RawJsonDto> filteredByStoreName = filterNullValues;
            if (!string.IsNullOrEmpty(request.Store))
            {
                var shopNameValidator = await _shopNameValidator.ValidateShopName(request.Store);
                filteredByStoreName = productFilterService.FilterByStoreName(filterNullValues, shopNameValidator);
            }

            var outOfStockFilter = productFilterService.OutOfStockFilter(filteredByStoreName);
            if (outOfStockFilter.Count == 0)
            {
                throw new OutOFStockExeption("No products in stock");
            }

            var groupByLowestPrice = productFilterService.GroupByLowestPrice(outOfStockFilter).ToList();
            var userId = userIdService.GetUserId();

            try
            {
                await productSaveService.SaveProductsAsync<List<RawJsonDto>>(groupByLowestPrice, userId, false);
            }
            catch (Exception)
            {
                throw new FailedToSaveExeption("Error occurred while saving products");
            }

            return new RawJsonDtoResponse { Data = groupByLowestPrice };
        }
    }
}
