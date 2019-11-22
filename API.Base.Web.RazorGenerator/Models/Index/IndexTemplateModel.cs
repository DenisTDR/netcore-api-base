using System.Collections.Generic;
using System.Reflection;
using API.Base.Web.Base.Models;

namespace API.Base.Web.RazorGenerator.Models.Index
{
    public class IndexTemplateModel : BaseTemplateModel
    {
        public IndexTemplateModel()
        {
        }

        public IndexTemplateModel(string fullTypeName, string entityName, string pageTitle)
        {
            FullTypeName = fullTypeName;
            EntityName = entityName;
            PageTitle = pageTitle;
        }

        public IList<PropertyInfo> Columns { get; set; }

        public IList<string> Actions { get; set; }

        public IList<AdminDashboardLink> TopLinks { get; set; }

        public bool IsOrdered { get; set; }
    }
}