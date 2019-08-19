using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API.Base.Web.Base.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetSubTypes(this Assembly assembly, Type type)
        {
            var isGenericType = type.IsGenericType;
            return assembly.GetTypes()
                .Where(
                    t => t != type && (isGenericType ? t.IsSubclassOfGenericType(type) : type.IsAssignableFrom(t)) &&
                         !t.GetTypeInfo().IsGenericType);
        }
    }
}