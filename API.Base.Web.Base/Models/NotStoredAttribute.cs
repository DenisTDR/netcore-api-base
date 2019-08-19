using System;

namespace API.Base.Web.Base.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NotStoredAttribute : Attribute
    {
    }
}