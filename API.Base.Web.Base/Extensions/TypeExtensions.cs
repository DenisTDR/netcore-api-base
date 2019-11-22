using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Base.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsSubclassOfGenericType(this Type type, Type genericType)
        {
            var tmpType = type;
            if (tmpType == genericType)
            {
                return false;
            }

            while (tmpType != null && tmpType != typeof(object))
            {
                var crt = tmpType.IsGenericType ? tmpType.GetGenericTypeDefinition() : tmpType;

                if (crt == genericType)
                {
                    return true;
                }

                tmpType = tmpType.BaseType;
            }

            return false;
        }

        public static bool ImplementsGenericInterface(this Type type, Type genericInterfaceType)
        {
            return type.GetInterfaces().Any(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == genericInterfaceType);
        }

        public static Type GetGenericArgumentTypeOfImplementedGenericInterface(this Type type,
            Type genericInterfaceType)
        {
            if (!type.ImplementsGenericInterface(genericInterfaceType))
            {
                return null;
            }

            var interfaceType = type.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericInterfaceType);
            return interfaceType?.GetGenericArguments()[0];
        }

        public static bool IsA(this Type type, Type parent)
        {
            if (type == parent)
            {
                return true;
            }

            return type.IsSubclassOfGenericType(parent);
        }


        public static bool IsEntity(this Type type)
        {
            return typeof(IEntity).IsAssignableFrom(type);
        }

        public static bool IsCollectionOfEntities(this Type type)
        {
            return type.IsCollectionType() && type.GenericTypeArguments.Length != 0 &&
                   type.GenericTypeArguments[0].IsEntity();
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsBooleanType() || type.IsStringType() || type.IsNumericType()
                   || typeof(DateTime).IsAssignableFrom(type) || typeof(TimeSpan).IsAssignableFrom(type);
        }

        public static bool IsBooleanType(this Type type)
        {
            return type.IsAssignableFrom(typeof(bool));
        }

        public static bool IsStringType(this Type type)
        {
            return type.IsAssignableFrom(typeof(string));
        }

        public static bool IsNumericType(this Type type)
        {
            if (type.IsEnum)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsCollectionType(this Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type) || typeof(IList).IsAssignableFrom(type)
                                                              || typeof(ICollection<>).IsAssignableFrom(type) ||
                                                              typeof(IList<>).IsAssignableFrom(type)
                                                              || typeof(IEnumerable).IsAssignableFrom(type) ||
                                                              typeof(IQueryable).IsAssignableFrom(type);
        }

        public static string CSharpName(this Type type)
        {
            var sb = new StringBuilder();
            var name = type.Name;
            if (!type.GetTypeInfo().IsGenericType) return name;
            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments()
                .Select(t => t.CSharpName())));
            sb.Append(">");
            return sb.ToString();
        }

        public static string GetDisplayDescription(this Enum value)
        {
            return value.GetDisplayProperty((attribute => attribute.Description));
        }

        public static string GetDisplayName(this Enum value)
        {
            return value.GetDisplayProperty((attribute => attribute.Name)) ?? value?.ToString();
        }

        private static string GetDisplayProperty(this Enum value, Func<DisplayAttribute, string> func)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null) return null;
            var field = type.GetField(name);
            if (field == null) return null;
            var attr = field.GetCustomAttributes<DisplayAttribute>().FirstOrDefault();
            return attr != null ? func(attr) : null;
        }
    }
}