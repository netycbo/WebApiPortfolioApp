using MediatR;
using WebApiPortfolioApp.API.Respons;

namespace WebApiPortfolioApp.API.Request
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
