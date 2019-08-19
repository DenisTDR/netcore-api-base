using System.Collections.Generic;

namespace API.Base.Web.RazorGenerator.Models.Details
{
    public class DetailsTemplateModel : BaseTemplateModel
    {
        public DetailsTemplateModel(string fullTypeName, string entityName, string pageTitle) : base(fullTypeName,
            entityName, pageTitle)
        {
        }

        public IList<string> Actions { get; set; }
    }
}