using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace WebApiPortfolioApp.Providers.ViewRender
{
    public class ViewRender
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ViewRender(IServiceScopeFactory serviceScopeFactory, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var httpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                using (var sw = new StringWriter())
                {
                    var viewResult = FindView(actionContext, viewName);
                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException($"{viewName} does not match any available view");
                    }

                    var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };

                    var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, scope.ServiceProvider.GetRequiredService<ITempDataProvider>()),
                        sw,
                        new HtmlHelperOptions()
                    );

                    await viewResult.View.RenderAsync(viewContext);
                    return sw.ToString();
                }
            }
        }

        private ViewEngineResult FindView(ActionContext actionContext, string viewName)
        {
            var engine = actionContext.HttpContext.RequestServices.GetRequiredService<IRazorViewEngine>();
            var result = engine.FindView(actionContext, viewName, false);
            if (!result.Success)
            {
                // Log result.SearchedLocations
                var searchedLocations = string.Join(", ", result.SearchedLocations);
                Console.WriteLine("Wyszukiwane lokalizacje: " + searchedLocations);
            }
            return result;
        }
    }

}
