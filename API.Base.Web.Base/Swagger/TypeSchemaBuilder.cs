using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Models.ViewModels;
using Swashbuckle.AspNetCore.Swagger;

namespace API.Base.Web.Base.Swagger
{
    internal class TypeSchemaBuilder
    {
        private static readonly ConcurrentDictionary<Type, Schema> Cache = new ConcurrentDictionary<Type, Schema>();

        public static IEnumerable<Tuple<string, Schema>> GetRegistered =>
            Cache.Select(kvp => new Tuple<string, Schema>(kvp.Key.Name, kvp.Value));

        public Schema Build(Type type)
        {
            if (Cache.ContainsKey(type))
            {
                return Cache[type];
            }

            Cache[type] = BuildSchema(type);
            return Cache[type];
        }

        public Schema Build<T>() where T : ViewModel
        {
            return Build(typeof(T));
        }

        private Schema BuildSchema(Type type)
        {
            var schema = new Schema
            {
                Properties = new Dictionary<string, Schema>(),
                Type = "object"
            };

            var fields = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fieldInfo in fields)
            {
                if (!fieldInfo.PropertyType.IsPrimitive())
                {
                    Console.WriteLine("found: " + fieldInfo.Name + " which is " + fieldInfo.PropertyType.FullName);
                    continue;
                }

                schema.Properties[fieldInfo.Name.ToLower()] = BuildSchemaForPrimitve(fieldInfo.PropertyType);
            }

            return schema;
        }

        private Schema BuildSchemaForPrimitve(Type type)
        {
            var schema = new Schema
            {
                Type = type.Name.ToLower()
            };
            if (type.IsNumericType())
            {
                schema.Type = "integer";
                schema.Format = type.Name.ToLower();
            }

            return schema;
        }
    }
}