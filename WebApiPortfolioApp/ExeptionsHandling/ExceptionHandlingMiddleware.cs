using System.Net;
using WebApiPortfolioApp.ExeptionsHandling.Exeptions;

namespace WebApiPortfolioApp.ExeptionsHandling
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                EmailNotUniqueException => (int)HttpStatusCode.Conflict,
                CantCreateUserExeption => (int)HttpStatusCode.Conflict,
                NoMatchingFiltredProductsExeption => (int)HttpStatusCode.Conflict,
                InvalidPasswordException => (int)HttpStatusCode.Conflict,
                UserNotFoundException => (int)HttpStatusCode.Conflict,
                UsernameAlreadyTakenException => (int)HttpStatusCode.Conflict,
                _ => (int)HttpStatusCode.InternalServerError
            };

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            }.ToString());
        }
    }
}

