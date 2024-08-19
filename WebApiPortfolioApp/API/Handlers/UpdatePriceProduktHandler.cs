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
    public class UpdatePriceProduktHandler(AppDbContext context, IApiCall apiCall, IMapper mapper,
        IDeserializeService deserializeService, ISaveProductService productSaveService, IProductFilterService productFilterService) : IRequestHandler<UpdatePriceProduktRequest, UpdatePriceProduktRespons>
    {
        public async Task<UpdatePriceProduktRespons> Handle(UpdatePriceProduktRequest request, CancellationToken cancellationToken)
        {
            var products = await context.ProductSubscriptions
                .Where(up => up.UserName == request.UserName)
                .Select(up => up.ProductName)
                .ToListAsync(cancellationToken);

            var updateResponses = new List<UpdatePriceProduktDto>();

            foreach (var product in products)
            {
                var restRequest = apiCall.CreateProductSearchRequest(product, 10);
                var response = await apiCall.ExecuteRequestAsync(restRequest, cancellationToken);
                Console.WriteLine($"Response Content: {response.Content}");

                if (response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    throw new FailedToFetchDataExeption("Failed to fetch data");
                }

                var rawProductResponse = deserializeService.Deserialize<RawJsonDtoResponse>(response.Content);
                if (rawProductResponse == null || rawProductResponse.Data == null)
                {
                    continue;
                }

                var mappedProducts = mapper.Map<List<RawJsonDto>>(rawProductResponse.Data);
                var filterNullValues = productFilterService.FilterNullValues(mappedProducts);
                if (filterNullValues.Count == 0)
                {
                    throw new NoMatchingFiltredProductsExeption("To many null values in data");
                }
                var groupedData = productFilterService.GroupByLowestPrice(filterNullValues);
                var updateProductDtos = mapper.Map<List<UpdatePriceProduktDto>>(groupedData);
                updateResponses.AddRange(updateProductDtos);

                foreach (var updateProductDto in updateResponses)
                {
                    var tempProduct = mapper.Map<TemporaryProductsDto>(updateProductDto);

                    try
                    {
                        await productSaveService.SaveTemporaryProductsAsync(new List<TemporaryProductsDto> { tempProduct });
                    }
                    catch (FailedToSaveExeption ex)
                    {
                        throw new FailedToSaveExeption($"Error occurred while saving products: {ex.Message}");
                    }
                }

                var temporaryProducts = await context.TemporaryProducts.ToListAsync();
                foreach (var tempProduct in temporaryProducts)
                {
                    var productToUpdate = await context.ProductSubscriptions.FirstOrDefaultAsync(
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

                context.SaveChanges();
                context.TemporaryProducts.RemoveRange(temporaryProducts);
            }

            context.SaveChanges();

            return new UpdatePriceProduktRespons
            {
                Data = updateResponses
            };
        }
    }
}
