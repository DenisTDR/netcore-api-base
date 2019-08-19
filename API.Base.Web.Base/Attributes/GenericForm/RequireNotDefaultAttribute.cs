using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Internal;

namespace API.Base.Web.Base.Attributes.GenericForm
{
    public class RequireNotDefaultAttribute : RequiredAttribute
    {
        private readonly Type _propertyType;

        public RequireNotDefaultAttribute(Type propertyType)
        {
            _propertyType = propertyType;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return false;
            return !_propertyType.IsDefaultValue(value);
        }
    }
}