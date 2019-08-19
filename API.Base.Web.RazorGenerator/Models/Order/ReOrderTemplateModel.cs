namespace API.Base.Web.RazorGenerator.Models.Order
{
    public class OrderTemplateModel : BaseTemplateModel
    {
        public OrderTemplateModel(string fullTypeName, string entityName, string pageTitle)
        {
            FullTypeName = fullTypeName;
            EntityName = entityName;
            PageTitle = pageTitle;
        }
    }
}