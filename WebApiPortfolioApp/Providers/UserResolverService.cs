namespace WebApiPortfolioApp.Providers
{
    public class UserResolverService
    {
        private readonly IHttpContextAccessor _context;

        public UserResolverService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUser()
        {
            var httpContext = _context.HttpContext;
            if (httpContext == null || httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            return httpContext.User.Identity.Name;
        }
    }
}
