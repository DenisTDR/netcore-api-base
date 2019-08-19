using System.Collections.Generic;

namespace API.Base.Web.RazorGenerator.Models
{
    public abstract class BaseTemplateModel
    {
        protected BaseTemplateModel()
        {
        }

        protected BaseTemplateModel(string fullTypeName, string entityName, string pageTitle)
        {
            FullTypeName = fullTypeName;
            EntityName = entityName;
            PageTitle = pageTitle;
        }

        public string FullTypeName { get; set; }
        public string EntityName { get; set; }
        public string PageTitle { get; set; }

        public bool ImportHtmlHelpers { get; set; }
        public bool InjectDataLayer { get; set; }
        
    }
}