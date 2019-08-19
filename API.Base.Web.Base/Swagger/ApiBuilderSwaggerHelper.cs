using API.Base.Web.Base.ApiBuilder;
using API.Base.Web.Base.Swagger.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace API.Base.Web.Base.Swagger
{
    public class ApiBuilderSwaggerHelper
    {
        private readonly SwaggerSpecs _swaggerSpecs;

        public ApiBuilderSwaggerHelper(SwaggerSpecs swaggerSpecs)
        {
            _swaggerSpecs = swaggerSpecs;
        }

        public void Bind(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger(c => { c.RouteTemplate = _swaggerSpecs.RouteTemplate; });

            var resAssembly = typeof(ApiBuilder.ApiBuilder).Assembly;

            if (_swaggerSpecs.IndexStreamAction == null)
            {
                var indexResName = resAssembly.GetName().Name + ".wwwroot.api.swagger.index.html";
                _swaggerSpecs.IndexStreamAction = () => resAssembly.GetManifestResourceStream(indexResName);
            }

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(uiOptions =>
            {
                uiOptions.SwaggerEndpoint("/api/docs/" + _swaggerSpecs.Name + "/swagger.json",
                    _swaggerSpecs.Title + " " + _swaggerSpecs.Version);
                uiOptions.RoutePrefix = "api/docs";
                uiOptions.InjectStylesheet("/api/swagger/swagger-ui-theme.css");
                uiOptions.InjectJavascript("/api/swagger/swagger-ui-theme.js");
                uiOptions.IndexStream = _swaggerSpecs.IndexStreamAction;
//                uiOptions.inde
            });
        }

        public void Bind(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(_swaggerSpecs.Name,
                    new Info {Title = _swaggerSpecs.Title, Version = _swaggerSpecs.Version});
                options.OperationFilter<SwaggerCustomOperationFilter>();
                options.DocumentFilter<SwaggerCustomDocumentFilter>();
                options.SchemaFilter<GenericFormConfigGenerator>();

//                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });
        }
    }
}