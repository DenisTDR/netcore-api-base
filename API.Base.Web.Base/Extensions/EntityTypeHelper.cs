using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.Base.Web.Base.Attributes;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Base.Extensions
{
    public class EntityTypeHelper
    {
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> UpdatablePrimitiveProperties =
            new ConcurrentDictionary<Type, List<PropertyInfo>>();

        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> UpdatableEntityProperties =
            new ConcurrentDictionary<Type, List<PropertyInfo>>();


        public static List<PropertyInfo> GetUpdatablePrimitveProperties<T>(IEnumerable<string> withNames = null)
            where T : IEntity
        {
            if (!UpdatablePrimitiveProperties.ContainsKey(typeof(T)))
            {
                UpdatablePrimitiveProperties[typeof(T)] = GetNotReadOnlyProps<T>()
                    .Where(prop => prop.PropertyType.IsPrimitive() || prop.PropertyType.IsEnum)
                    .ToList();
            }

            var result = UpdatablePrimitiveProperties[typeof(T)];
            if (withNames != null)
            {
                result = result.Where(p => withNames.Contains(p.Name.ToLower())).ToList();
            }

            return result;
        }


        public static List<PropertyInfo> GetUpdateableEntityProperties<T>(IEnumerable<string> withNames = null)
            where T : IEntity
        {
            if (!UpdatableEntityProperties.ContainsKey(typeof(T)))
            {
                UpdatableEntityProperties[typeof(T)] =
                    GetNotReadOnlyProps<T>()
                        .Where(p => p.PropertyType.IsEntity())
                        .ToList();
            }

            var updatableProps = UpdatableEntityProperties[typeof(T)];
            if (withNames != null)
            {
                updatableProps = updatableProps.Where(p => withNames.Contains(p.Name.ToLower())).ToList();
            }

            return updatableProps.ToList();
        }


        private static IEnumerable<PropertyInfo> GetNotReadOnlyProps<T>(bool includeReadOnly = false)
        {
            return typeof(T).GetProperties()
                .Where(prop => includeReadOnly || prop.GetCustomAttributes().All(attr =>
                                   !(attr is IsReadOnlyAttribute && ((IsReadOnlyAttribute) attr).Is)));
        }

        public static IEnumerable<string> GetPropertiesNames(Dictionary<string, bool> properties)
        {
            return properties.Where(kvp => kvp.Value).Select(kvp => kvp.Key.ToLower());
        }
    }
}