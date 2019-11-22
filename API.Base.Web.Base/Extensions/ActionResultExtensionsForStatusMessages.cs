using System.Threading.Tasks;
using API.Base.Web.Base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Base.Extensions
{
    public static class ActionResultExtensionsForStatusMessages
    {
        public static IActionResult WithSuccess(this IActionResult result, string title, string body)
        {
            return WithStatusMessage(result, "success", title, body);
        }

        public static IActionResult WithInfo(this IActionResult result, string title, string body)
        {
            return WithStatusMessage(result, "info", title, body);
        }

        public static IActionResult WithWarning(this IActionResult result, string title, string body)
        {
            return WithStatusMessage(result, "warning", title, body);
        }

        public static IActionResult WithDanger(this IActionResult result, string title, string body)
        {
            return WithStatusMessage(result, "danger", title, body);
        }

        public static IActionResult WithStatusMessage(IActionResult result, string type, string title, string body)
        {
            return new StatusMessageDecoratorResult(result, type, title, body);
        }
    }

    public class StatusMessageDecoratorResult : IActionResult
    {
        public IActionResult Result { get; }
        public string Type { get; }
        public string Title { get; }
        public string Body { get; }

        public StatusMessageDecoratorResult(IActionResult result, string type, string title, string body)
        {
            Result = result;
            Type = type;
            Title = title;
            Body = body;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();

            var tempData = factory.GetTempData(context.HttpContext);
            tempData["_statusMessage"] = new StatusMessageWithType(Title, Body, Type);

            await Result.ExecuteResultAsync(context);
        }
    }
}