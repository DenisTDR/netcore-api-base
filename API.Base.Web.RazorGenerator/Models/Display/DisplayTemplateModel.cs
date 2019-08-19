using System.Collections.Generic;
using System.Reflection;

namespace API.Base.Web.RazorGenerator.Models.Display
{
    public class DisplayTemplateModel : BaseTemplateModel
    {
        public DisplayTemplateModel()
        {
        }

        public DisplayTemplateModel(string fullTypeName, string entityName, string pageTitle) : base(fullTypeName, entityName, pageTitle)
        {
        }

        public IList<PropertyInfo> Properties { get; set; }
    }
}