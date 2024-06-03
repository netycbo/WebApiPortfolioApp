using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data;
using System.Linq;

namespace WebApiPortfolioApp.API.Handlers
{
    public class GetAllProductsNameHandler : IRequestHandler<GetAllProductsNameRequest, GetAllProductsNameRespons>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public GetAllProductsNameHandler(IMapper mapper, AppDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<GetAllProductsNameRespons> Handle(GetAllProductsNameRequest request, CancellationToken cancellationToken)
        {
            var searchStrings = await _dbContext.SearchHistory
                .Select(sh => sh.SearchString)
                .ToListAsync(cancellationToken);

            // Normalizing the product names by sorting words within each name
            var productNameCounts = new Dictionary<string, int>();
            foreach (var productName in searchStrings)
            {
                var normalizedProductName = String.Join(" ", productName.Split(' ').OrderBy(word => word).ToArray());
                if (productNameCounts.ContainsKey(normalizedProductName))
                {
                    productNameCounts[normalizedProductName]++;
                }
                else
                {
                    productNameCounts.Add(normalizedProductName, 1);
                }
            }

            // Mapping the dictionary to a list of ProductNamesDto, including the count
            var productNames = productNameCounts.Select(pn => new ProductNamesDto
            {
                ProductName = pn.Key, // This will be the normalized name
                Quantity = pn.Value
            }).ToList();

            return new GetAllProductsNameRespons
            {
                Data = productNames
            };
        }
    }
}
