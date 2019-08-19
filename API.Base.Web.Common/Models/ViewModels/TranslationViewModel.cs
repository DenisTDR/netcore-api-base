using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Web.Common.Models.ViewModels
{
    public class TranslationViewModel : ViewModel
    {
        public string Slug { get; set; }
        public string Value { get; set; }
    }
}