using System.Linq;
using API.Base.Web.Base.Attributes;
using API.Base.Web.Base.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Base.Web.Base.Filters
{
    internal class ResultWrapperFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {        
            if (context.Filters.OfType<NoResultWrapperAttribute>().Any())
            {
                return;
            }
            
            if (context.Result is ObjectResult objectResult)
            {
                var oldValue = objectResult.Value;
                var newValue = new DataResponseModel(oldValue);
                objectResult.Value = newValue;
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}