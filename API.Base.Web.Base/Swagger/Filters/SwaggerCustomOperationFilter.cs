using System;
using API.Base.Web.Base.Models.ViewModels;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Base.Web.Base.Swagger.Filters
{
    internal class SwaggerCustomOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            foreach (var responsesValue in operation.Responses.Values)
            {
                Schema schemaToRepair = null;
                if (responsesValue?.Schema?.Ref == "#/definitions/ViewModel")
                {
                    schemaToRepair = responsesValue.Schema;
                }
                else if (responsesValue?.Schema?.Items?.Ref == "#/definitions/ViewModel")
                {
                    schemaToRepair = responsesValue.Schema.Items;
                }

                var dType = context.MethodInfo.DeclaringType;
                if (schemaToRepair != null)
                {
                    if (!TryRepair(schemaToRepair, dType))
                    {
                        TryRepair(schemaToRepair, dType.BaseType);
                    }
                }
            }
        }

        private bool TryRepair(Schema schema, Type dType)
        {
            if (dType.GenericTypeArguments.Length == 2 &&
                typeof(ViewModel).IsAssignableFrom(dType.GenericTypeArguments[1]))
            {
                schema.Ref = "#/definitions/" + dType.GenericTypeArguments[1].Name;
                return true;
            }

            return false;
        }
    }
}