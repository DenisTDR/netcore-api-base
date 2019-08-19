using System.Collections.Generic;
using System.Reflection;

namespace API.Base.Web.RazorGenerator.Models.Form
{
    public class FormTemplateModel : BaseTemplateModel
    {
        public FormTemplateModel()
        {
        }

        public FormTemplateModel(string fullTypeName, string entityName, string pageTitle) : base(fullTypeName, entityName,
            pageTitle)
        {
        }

        public IList<PropertyInfo> Properties { get; set; }
    }
}