using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Base.Web.Base.Attributes
{
    public class NoResultWrapperAttribute : Attribute, IFilterMetadata
    {
    }
}
