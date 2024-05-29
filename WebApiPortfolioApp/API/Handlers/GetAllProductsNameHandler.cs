using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiPortfolioApp.API.DTOs;
using WebApiPortfolioApp.API.Request;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data;

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

            var productNames = _mapper.Map<List<ProductNamesDto>>(searchStrings);

            return new GetAllProductsNameRespons
            {
                Data = productNames
            };
        }
    }
}
