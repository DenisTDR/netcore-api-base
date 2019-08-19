using System;

namespace API.Base.Web.Base.Attributes.GenericForm
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FieldAutoSetFieldAttribute : Attribute
    {
        public string From { get; }
        public string To { get; }

        public FieldAutoSetFieldAttribute(string from, string to)
        {
            From = from;
            To = to;
        }
    }
}