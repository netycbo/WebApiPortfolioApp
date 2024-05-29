using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApiPortfolioApp.API.Respons;
using WebApiPortfolioApp.Data;

namespace WebApiPortfolioApp.API.Request
{
    public class GetAllProductsNameRequest : IRequest<GetAllProductsNameRespons>
    {
       
    }
}
