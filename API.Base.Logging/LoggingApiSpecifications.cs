using System;
using API.Base.Logging.Filters;
using API.Base.Logging.Logger;
using API.Base.Web.Base.ApiBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Logging
{
    public class LoggingApiSpecifications : ApiSpecifications
    {
        public override void ConfigMvc(MvcOptions options)
        {
            options.Filters.Add<ActionLogFilter>();
            options.Filters.Add<ExceptionLogFilter>();
            options.Filters.Add<AdminLogFilter>();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILLogger, LLogger>();
        }

        public override void ConfigureApp(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            base.ConfigureApp(app, serviceProvider);
            var logger = serviceProvider.GetService<ILLogger>();
            logger.LogInfo("Application started");
        }
    }
}