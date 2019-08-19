using System.Collections.Generic;
using System.Reflection;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.RazorGenerator.Models.Display
{
    public interface IGenerableDisplayView<TE> : IGenerableView<TE> where TE : Entity
    {
        IList<PropertyInfo> DisplayProperties { get; set; }
    }
}