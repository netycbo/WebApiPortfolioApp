using System.Security.Claims;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;

namespace WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices
{
    public class UserIdService : IUserIdService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return -1; // Zwrot wartości -1, gdy użytkownik nie jest zalogowany
            }

            // Pobieranie identyfikatora użytkownika z roszczeń (claims)
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : -1;

            
        }
    }
}
