using API.Base.Web.Base.Models;
using API.Base.Web.Base.Models.ViewModels;
using API.Base.Web.Common.Models.Entities;

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