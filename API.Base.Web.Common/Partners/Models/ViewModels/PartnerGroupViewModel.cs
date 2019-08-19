using System.Collections.Generic;
using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Web.Common.Partners.Models.ViewModels
{
    public class PartnerGroupViewModel : ViewModel
    {
        public string Name { get; set; }
        public virtual List<PartnerViewModel> Partners { get; set; }
        public int LogoSize { get; set; }
    }
}