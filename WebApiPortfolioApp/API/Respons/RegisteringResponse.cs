using WebApiPortfolioApp.API.DTOs;

namespace WebApiPortfolioApp.API.Respons
{
    public class RegisteringResponse : BaseResponse<RegisteringDto>
    {
        public string Error { get; set; }
    }
}
