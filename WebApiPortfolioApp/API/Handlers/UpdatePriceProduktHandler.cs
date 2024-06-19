using MediatR;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;
using RestSharp;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
using System.Linq;
using System.Threading.Tasks;
using WebApiPortfolioApp.Data.Entinities;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;

namespace WebApiPortfolioApp.API.Handlers
{
    public class UpdatePriceProduktHandler : IRequestHandler<UpdatePriceProduktRequest, UpdatePriceProduktRespons>
    {
        private readonly AppDbContext _context;
        private readonly IApiCall _apiCall;
        private readonly IMapper _mapper;
        private readonly IDeserializeService _deserializeService;
        private readonly ISaveProductService _productSaveService;

        public UpdatePriceProduktHandler(AppDbContext context, IApiCall apiCall, IMapper mapper,
            IDeserializeService deserializeService, ISaveProductService productSaveService)
        {
            _context = context;
            _apiCall = apiCall;
            _mapper = mapper;
            _deserializeService = deserializeService;
            _productSaveService = productSaveService;
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

                var filteredData = rawProductResponse.Data
                    .Where(d => d.Name == product && d.Current_Price != null && d.Current_Price != 0)
                    .ToList();
                var groupedData = filteredData
                    .GroupBy(d => d.Name)
                    .Select(g => g.OrderBy(d => d.Current_Price).FirstOrDefault())
                    .ToList();

                foreach (var rawDto in groupedData)
                {
                    var updateProductDto = new UpdatePriceProduktDto
                    {
                        ProductName = rawDto.Name,
                        Price = rawDto.Current_Price,
                        Store = rawDto.Store
                    };

                    var tempProduct = _mapper.Map<TemporaryProduct>(updateProductDto);
                    updateResponses.Add(updateProductDto);
                    Console.WriteLine($"TemporaryProduct: Name = {tempProduct.Name}, Price = {tempProduct.Price}");

                    try
                    {
                        await _productSaveService.SaveTemporaryProductsAsync(new List<TemporaryProduct> { tempProduct });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving temporary product: {ex.Message}");
                        throw;
                    }
                }

                var temporaryProducts = await _context.TemporaryProducts.ToListAsync();
                foreach (var tempProduct in temporaryProducts)
                {
                    var productToUpdate = await _context.ProductSubscriptions.FirstOrDefaultAsync(p => p.ProductName == tempProduct.Name);
                    if (productToUpdate == null)
                    {
                        continue;
                    }

                    if (productToUpdate.Price < tempProduct.Price)
                    {
                        productToUpdate.Price = tempProduct.Price;
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
