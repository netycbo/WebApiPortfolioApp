namespace WebApiPortfolioApp.Providers.ViewRender
{
    public interface IViewRender
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
