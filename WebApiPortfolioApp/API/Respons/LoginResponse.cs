using WebApiPortfolioApp.API.DTOs;

namespace WebApiPortfolioApp.API.Respons
{
    public class LoginResponse : BaseResponse<LoginDto>
    {
        public string Message { get; set; }
        public string Token { get; set; }
    }
}
