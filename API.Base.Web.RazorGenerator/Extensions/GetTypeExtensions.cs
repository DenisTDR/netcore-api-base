using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API.Base.Web.RazorGenerator.Extensions
{
    public static class GetTypeExtensions
    {
        public static Type GetType(string typeName)
        {
            var all = GetAllAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .ToList();
            var exactType = all.FirstOrDefault(type => type.FullName == typeName);
            if (exactType != null)
            {
                return exactType;
            }

            var foundTypes = all.Where(type => type.Name == typeName).ToList();
            if (foundTypes.Count != 1)
            {
                return null;
            }

            return foundTypes.FirstOrDefault();
        }


        private static IEnumerable<Assembly> GetAllAssemblies()
        {
            var assemblies = Assembly
                .GetEntryAssembly()
                .GetReferencedAssemblies().ToList();
            assemblies.Add(Assembly.GetEntryAssembly().GetName());
            return assemblies.Select(Assembly.Load);
        }
    }
}