using System.Collections.Generic;
using System.Reflection;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Models;
using API.Base.Web.Common.Controllers.Ui.Nv;
using API.Base.Web.Common.Partners.Models.Entities;

namespace API.Base.Web.Common.Partners.Controllers
{
    public class PartnerTypesUiController : NvGenericUiController<PartnerTypeEntity>
    {
        protected override void SetListColumns()
        {
            ListColumns.AddRange(GetRelevantProperties());
        }

        protected override void SetFormProperties()
        {
            FormProperties.AddRange(GetRelevantProperties());
        }

        private IEnumerable<PropertyInfo> GetRelevantProperties()
        {
            return new[]
            {
                typeof(PartnerTypeEntity).GetProperty(nameof(PartnerTypeEntity.Name)),
                typeof(PartnerTypeEntity).GetProperty(nameof(PartnerTypeEntity.LogoSize))
            };
        }

        protected override void SetTopLinks()
        {
            TopLinks.Add(new AdminDashboardLink("Partners", typeof(PartnersUiController)));
            TopLinks.Add(new AdminDashboardLink("Partner Tiers", typeof(PartnerTiersUiController)));
        }
    }
}