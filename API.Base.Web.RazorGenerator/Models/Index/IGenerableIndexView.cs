using System.Collections.Generic;
using System.Reflection;
using API.Base.Web.Base.Models;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.RazorGenerator.Models.Index
{
    public interface IGenerableIndexView<TE> : IGenerableView<TE> where TE : Entity
    {
        IList<AdminDashboardLink> TopLinks { get; set; }
        IList<string> ListItemActions { get; set; }
        IList<PropertyInfo> ListColumns { get; set; }
    }
}