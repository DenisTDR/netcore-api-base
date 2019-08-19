using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Base.Web.Base.Swagger.Filters
{
    internal class SwaggerCustomDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths = swaggerDoc.Paths.Where(kvp => !kvp.Key.EndsWith("IgnoreThisEndpoint"))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}