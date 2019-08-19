using System.Collections.Generic;

namespace API.Base.Web.Common.Partners.Models.ViewModels
{
    public class PartnerTypeViewModel : PartnerGroupViewModel
    {
        public virtual List<PartnerTierViewModel> Tiers { get; set; }
    }
}