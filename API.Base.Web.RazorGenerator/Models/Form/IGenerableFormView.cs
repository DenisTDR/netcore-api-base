using System.Collections.Generic;
using System.Reflection;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.RazorGenerator.Models.Form
{
    public interface IGenerableFormView<TE> : IGenerableView<TE> where TE : Entity
    {
        IList<PropertyInfo> FormProperties { get; set; }
    }
}