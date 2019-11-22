using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Web.Common.Models.ViewModels
{
    public class TemplateViewModel : ViewModel
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
    }
}