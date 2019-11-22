using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace API.Base.Web.RazorGenerator.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
        Task<string> RenderToStringAsync(string viewName, object model, ActionContext actionContext);
    }

    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        private IView GetView(ActionContext actionContext, string viewName)
        {
            var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"{viewName} does not match any available view," +
                                                JsonConvert.SerializeObject(viewResult.SearchedLocations));
            }

            return viewResult.View;
        }

        public Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext {RequestServices = _serviceProvider};
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            return RenderToStringAsync(viewName, model, actionContext);
        }

        public async Task<string> RenderToStringAsync(string viewName, object model, ActionContext actionContext)
        {
            using (var sw = new StringWriter())
            {
                var view = GetView(actionContext, viewName);

                var viewDictionary =
                    new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };
                

                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await view.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}