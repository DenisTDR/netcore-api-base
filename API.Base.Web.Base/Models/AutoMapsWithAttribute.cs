using System;

namespace API.Base.Web.Base.Models
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AutoMapsWithAttribute : Attribute
    {
        public Type TargetType { get; private set; }

        public AutoMapsWithAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}