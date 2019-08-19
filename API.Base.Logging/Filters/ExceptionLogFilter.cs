using System;
using System.Collections.Generic;
using API.Base.Logging.Logger;
using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Misc.PatchBag;
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

            var method = context.HttpContext.Request.Method;
            if (method == "GET" || method == "OPTIONS")
            {
                return;
            }

            var epb = new EntityPatchBag<AuditEntity>
            {
                Id = context.HttpContext.TraceIdentifier,
                Model = new AuditEntity
                {
                    Data = context.Exception.ToString()
                },
                PropertiesToUpdate = new Dictionary<string, bool> {{"data", true}}
            };

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