using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Base.Web.Base.Swagger.Filters
{
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is IAllowAnonymousFilter);

            if (!isAuthorized || allowAnonymous) return;

            operation.Security = new List<IDictionary<string, IEnumerable<string>>> {_bearerSecurity};
        }

        private readonly Dictionary<string, IEnumerable<string>> _bearerSecurity =
            new Dictionary<string, IEnumerable<string>> {{"Bearer", new string[] { }}};
    }
}