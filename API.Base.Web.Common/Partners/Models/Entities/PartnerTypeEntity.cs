using System.Collections.Generic;

namespace API.Base.Web.Common.Partners.Models.Entities
{
    public class PartnerTypeEntity : PartnerGroupEntity
    {
        public virtual List<PartnerTierEntity> Tiers { get; set; }
    }
}