using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Web.Common.OgMetadata
{
    public class OgMetadataViewModel : ViewModel
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public OgMetadataType Type { get; set; }
        public string Image { get; set; }
    }
}