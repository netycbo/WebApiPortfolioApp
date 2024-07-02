using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.DTOs.Helpers;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;

namespace WebApiPortfolioApp.API.Handlers
{
    public class UpdatePriceProduktHandler : IRequestHandler<UpdatePriceProduktRequest, UpdatePriceProduktRespons>
    {
        private readonly AppDbContext _context;
        private readonly IApiCall _apiCall;
        private readonly IMapper _mapper;
        private readonly IDeserializeService _deserializeService;
        private readonly ISaveProductService _productSaveService;
        private readonly IProductFilterService _productFilterService;

        public UpdatePriceProduktHandler(AppDbContext context, IApiCall apiCall, IMapper mapper,
            IDeserializeService deserializeService, ISaveProductService productSaveService, IProductFilterService productFilterService)
        {
            _context = context;
            _apiCall = apiCall;
            _mapper = mapper;
            _deserializeService = deserializeService;
            _productSaveService = productSaveService;
            _productFilterService = productFilterService;
        }

        public async Task<UpdatePriceProduktRespons> Handle(UpdatePriceProduktRequest request, CancellationToken cancellationToken)
        {
            var products = await _context.ProductSubscriptions
                .Where(up => up.UserName == request.UserName)
                .Select(up => up.ProductName)
                .ToListAsync(cancellationToken);

            var updateResponses = new List<UpdatePriceProduktDto>();

            foreach (var product in products)
            {
                var restRequest = _apiCall.CreateProductSearchRequest(product, 10);
                var response = await _apiCall.ExecuteRequestAsync(restRequest, cancellationToken);
                Console.WriteLine($"Response Content: {response.Content}");

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    throw new FailedToFetchDataExeption("Failed to fetch data");
                }

                var rawProductResponse = _deserializeService.Deserialize<RawJsonDtoResponse>(response.Content);
                if (rawProductResponse == null || rawProductResponse.Data == null)
                {
                    continue;
                }

                var mappedProducts = _mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);
                var filterNullValues = _productFilterService.FilterNullValues(mappedProducts);
                if (filterNullValues.Count == 0)
                {
                    throw new NoMatchingFiltredProductsExeption("To many null values in data");
                }
                var groupedData = _productFilterService.GroupByLowestPrice(filterNullValues);
                var updateProductDtos = _mapper.Map<List<UpdatePriceProduktDto>>(groupedData);
                updateResponses.AddRange(updateProductDtos);

                foreach (var updateProductDto in updateResponses)
                {
                    var tempProduct = _mapper.Map<TemporaryProductsDto>(updateProductDto);

                    try
                    {
                        await _productSaveService.SaveTemporaryProductsAsync(new List<TemporaryProductsDto> { tempProduct });
                    }
                    catch (FailedToSaveExeption ex)
                    {
                        throw new FailedToSaveExeption($"Error occurred while saving products: {ex.Message}");
                    }
                }

                var temporaryProducts = await _context.TemporaryProducts.ToListAsync();
                foreach (var tempProduct in temporaryProducts)
                {
                    var productToUpdate = await _context.ProductSubscriptions.FirstOrDefaultAsync(
                        p => p.ProductName == tempProduct.Name);
                    if (productToUpdate == null)
                    {
                        continue;
                    }

                    if (productToUpdate.Price > tempProduct.Price)
                    {
                        productToUpdate.Price = tempProduct.Price;
                        productToUpdate.Store = tempProduct.Store;
                    }
                }

                _context.SaveChanges();
                _context.TemporaryProducts.RemoveRange(temporaryProducts);
            }

            _context.SaveChanges();

            return new UpdatePriceProduktRespons
            {
                Data = updateResponses
            };
        }
    }
}
