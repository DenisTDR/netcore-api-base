using System;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Base.Exceptions;
using API.Base.Web.Base.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace API.Base.Web.Base.Filters
{
    internal class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (IsApiController(context))
            {
                var responseModel = new DataResponseModel {Data = context.Exception?.Message};
                var dr = new ObjectResult(responseModel);

                if (context.Exception is KnownException knownExc)
                {
                    dr.StatusCode = knownExc.Code != 0 ? knownExc.Code : 500;
                }
                else
                {
                    Console.WriteLine("exception: " + context.Exception?.Message + " " + typeof(KnownException).Name);
                    dr.StatusCode = 500;
                }

                context.Result = dr;
            }
            else
            {
                if (context.Exception is KnownException exc)
                {
                    Console.WriteLine("CustomExceptionFilter.OnException: " + exc.Message);
//                    var x = context.Result;
//                    var vr = new ObjectResult(123);// {ViewName = "home/privacy"};
//                    //                    vr.ViewData = new ViewDataDictionary(context.cont);
//                    context.Result = vr;
                }
            }
        }

        public bool IsApiController(ExceptionContext context)
        {
            return context.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                   typeof(ApiController).IsAssignableFrom(actionDescriptor.ControllerTypeInfo);
        }
    }
}