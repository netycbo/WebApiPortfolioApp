using Microsoft.AspNet.Identity;
using System.Security.Claims;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class UserNameClaimService : IUserNameClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserNameClaimService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetUserName()
        {
            var userName = _httpContextAccessor.HttpContext?.User;
            var userNameClaim = userName.FindFirst(ClaimTypes.Name);
            if (userNameClaim == null)
            {
                return "User not exist";
            }
            return userNameClaim.Value;

        }
    }
}
