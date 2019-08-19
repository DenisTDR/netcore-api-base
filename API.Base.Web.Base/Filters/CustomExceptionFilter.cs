using System;
using API.Base.Web.Base.Exceptions;
using API.Base.Web.Base.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Base.Web.Base.Filters
{
    internal class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
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
    }
}