using API.Base.Files.Models.ViewModels;
using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Web.Common.Partners.Models.ViewModels
{
    public class PartnerViewModel : ViewModel
    {
        public string Name { get; set; }
        public FileViewModel Logo { get; set; }
        public PartnerTypeViewModel Type { get; set; }
        public PartnerTierViewModel Tier { get; set; }
        public string Url { get; set; }
        public bool Published { get; set; }
    }
}