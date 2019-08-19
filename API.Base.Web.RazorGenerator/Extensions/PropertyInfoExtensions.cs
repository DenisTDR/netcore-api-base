using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace API.Base.Web.RazorGenerator.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool HasDataType(this PropertyInfo propertyInfo, DataType dataType)
        {
            return propertyInfo.GetCustomAttributes<DataTypeAttribute>().FirstOrDefault()?.DataType == dataType;
        }
    }
}