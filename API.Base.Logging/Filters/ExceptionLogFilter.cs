using System;
using System.Collections.Generic;
using API.Base.Logging.Logger;
using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Base.Controllers.Ui;
using API.Base.Web.Base.Misc.PatchBag;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;

namespace API.Base.Logging.Filters
{
    internal class ExceptionLogFilter : IExceptionFilter
    {
        private readonly ILLogger _logger;
        private readonly IHostingEnvironment _environment;

        public ExceptionLogFilter(ILLogger logger, IHostingEnvironment environment)
        {
//            Console.WriteLine("ExceptionLogFilter ctor");
            _logger = logger;
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            if (!_environment.IsProduction())
            {
                Console.WriteLine("ExceptionLogFilter OnException: ");
                Console.WriteLine(context.Exception);
            }

            if (IsApiController(context))
            {
                OnApiException(context);
            }

            if (IsUiController(context))
            {
                OnUiException(context);
            }
        }

        public bool IsApiController(ExceptionContext context)
        {
            return context.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                   typeof(ApiController).IsAssignableFrom(actionDescriptor.ControllerTypeInfo);
        }

        public bool IsUiController(ExceptionContext context)
        {
            return context.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                   typeof(UiController).IsAssignableFrom(actionDescriptor.ControllerTypeInfo);
        }

        public void OnUiException(ExceptionContext context)
        {
            Console.WriteLine("OnUiException");

            var epb = new EntityPatchBag<LogsUiEntity>
            {
                Id = context.HttpContext.TraceIdentifier,
                Model = new LogsUiEntity
                {
                    OldVersion = "!!Exception occured!!!",
                    NewVersion = context.Exception.ToString()
                },
                PropertiesToUpdate = new Dictionary<string, bool> {{"OldVersion", true}, {"NewVersion", true}}
            };
            _logger.UpdateUiLog(epb);
        }

        public void OnApiException(ExceptionContext context)
        {
            var method = context.HttpContext.Request.Method;
            if (method == "GET" || method == "OPTIONS")
            {
                return;
            }

            var epb = new EntityPatchBag<LogsAuditEntity>
            {
                Id = context.HttpContext.TraceIdentifier,
                Model = new LogsAuditEntity
                {
                    Data = context.Exception.ToString()
                },
                PropertiesToUpdate = new Dictionary<string, bool> {{"data", true}}
            };
//
//            if (context.Exception is KnownException knownExc)
//            {
//                dr.StatusCode = knownExc.Code != 0 ? knownExc.Code : 500;
//            }
//            else
//            {
//                Console.WriteLine("exception: " + context.Exception?.Message + " " + typeof(KnownException).Name);
//                dr.StatusCode = 500;
//            }
            _logger.UpdateAuditLog(epb);
        }
    }
}