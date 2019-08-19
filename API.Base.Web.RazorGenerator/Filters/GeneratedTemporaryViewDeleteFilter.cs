using API.Base.Web.RazorGenerator.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Base.Web.RazorGenerator.Filters
{
    public class GeneratedTemporaryViewDeleteFilter : IResourceFilter
    {
        private IUiViewGeneratorService ViewGeneratorService { get; }

        public GeneratedTemporaryViewDeleteFilter(IUiViewGeneratorService viewGeneratorService)
        {
            ViewGeneratorService = viewGeneratorService;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            ViewGeneratorService.DeleteGeneratedView();
        }
    }
}