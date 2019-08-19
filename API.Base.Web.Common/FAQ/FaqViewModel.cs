using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Web.Common.FAQ
{
    public class FaqViewModel : ViewModel
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Category { get; set; }
        public bool Published { get; set; }
    }
}