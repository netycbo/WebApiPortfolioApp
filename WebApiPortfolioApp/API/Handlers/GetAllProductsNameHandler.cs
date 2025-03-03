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
            var searchStrings = await _dbContext.SearchHistories
                .Select(sh => sh.SearchString)
                .ToListAsync(cancellationToken);

            var productNameCounts = searchStrings
                .Select(productName => String.Join(" ", productName.Split(' ').OrderBy(word => word).ToArray()))
                .GroupBy(normalizedProductName => normalizedProductName)
                .ToDictionary(group => group.Key, group => group.Count());

            var productNames = productNameCounts.Select(pn => new ProductNamesDto
            {
                ProductName = pn.Key,
                Quantity = pn.Value
            }).ToList();

            return new GetAllProductsNameRespons
            {
                Data = productNames
            };
        }
    }
}
