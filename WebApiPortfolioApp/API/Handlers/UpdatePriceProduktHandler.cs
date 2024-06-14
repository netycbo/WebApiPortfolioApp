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
        private readonly TemporaryDbContext _temporaryDbContext;
        private readonly ISaveProductService _productSaveService;

        public UpdatePriceProduktHandler(AppDbContext context, IApiCall apiCall, IMapper mapper,
            IDeserializeService deserializeService, TemporaryDbContext temporaryDbContext, ISaveProductService productSaveService)
        {
            _context = context;
            _apiCall = apiCall;
            _mapper = mapper;
            _deserializeService = deserializeService;
            _temporaryDbContext = temporaryDbContext;
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
                var restRequest = _apiCall.CreateProductSearchRequest(product, 1);
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
                foreach (var rawDto in rawProductResponse.Data)
                {
                    var updateProductDto = new UpdatePriceProduktDto
                    {
                        ProductName = rawDto.Name,
                        Price = rawDto.Current_Price
                    };
                    Console.WriteLine($"Mapped UpdatePriceProduktDto: ProductName = {updateProductDto.ProductName}, Price = {updateProductDto.Price}");
                    updateResponses.Add(updateProductDto);
                    var tempProduct = _mapper.Map<TemporaryProduct>(updateProductDto);
                    Console.WriteLine($"Mapped TemporaryProduct: Name = {tempProduct.Name}, Price = {tempProduct.Price}");
                    await _productSaveService.SaveTemporaryProductsAsync(new List<TemporaryProduct> { tempProduct });
                }

                foreach (var tempProduct in _temporaryDbContext.TemporaryProducts)
                {
                    var productToUpdate = await _context.ProductSubscriptions.FirstOrDefaultAsync(p => p.ProductName == tempProduct.Name);

                    if (productToUpdate != null)
                    {
                        var searchHistoryEntry = await _context.SearchHistory.FirstOrDefaultAsync(sh =>
                            sh.UserId == productToUpdate.UserName && 
                            sh.SearchString == productToUpdate.ProductName);

                        if (searchHistoryEntry != null)
                        {
                            if (searchHistoryEntry.Price < tempProduct.Price)
                            {
                                searchHistoryEntry.Price = tempProduct.Price;
                            }
                        }
                    }
                }
                _context.SaveChanges();
                _temporaryDbContext.TemporaryProducts.RemoveRange(_temporaryDbContext.TemporaryProducts); 
            }
            return new UpdatePriceProduktRespons
            {
                Data = updateResponses
            };
        }
    }
}
