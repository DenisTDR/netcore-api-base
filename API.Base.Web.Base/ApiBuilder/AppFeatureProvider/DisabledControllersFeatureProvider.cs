using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.Base.Web.Base.Auth.Controllers;
using API.Base.Web.Base.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace API.Base.Web.Base.ApiBuilder.AppFeatureProvider
{
    // https://stackoverflow.com/a/46588830
    public class DisabledControllersFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IList<EntityTypeStackConfiguration> _entityStackConfigs;

        public DisabledControllersFeatureProvider(List<EntityTypeStackConfiguration> entityStackConfigs)
        {
            _entityStackConfigs = entityStackConfigs;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // https://stackoverflow.com/a/46588830

            // remove disabled controllers
            foreach (var config in _entityStackConfigs.Where(c => c.Disabled))
            {
                if (feature.Controllers.Contains(config.ApiControllerType?.GetTypeInfo()))
                {
                    feature.Controllers.Remove(config.ApiControllerType.GetTypeInfo());
                }

                if (feature.Controllers.Contains(config.UiControllerType?.GetTypeInfo()))
                {
                    feature.Controllers.Remove(config.UiControllerType.GetTypeInfo());
                }
            }

            // remove Base AuthController if custom one is defined
            var hasCustomAuthController = feature.Controllers.Any(c =>
                c != typeof(AuthController) && c.IsSubclassOfGenericType(typeof(AuthBasicController<>)));
            if (hasCustomAuthController)
            {
                feature.Controllers.Remove(typeof(AuthController).GetTypeInfo());
            }
        }
    }
}