using System;
using API.Base.Web.Base.Attributes.Base;

namespace API.Base.Web.Base.Attributes.GenericForm
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FieldBasicAttribute : BasicAttribute
    {
        public FieldBasicAttribute(string name, object value) : base(name, value)
        {
        }
    }
}