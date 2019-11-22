using System.Collections.Generic;
using System.Reflection;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Models;
using API.Base.Web.Common.Controllers.Ui.Nv;
using API.Base.Web.Common.Partners.Models.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Common.Partners.Controllers
{
    public class PartnerTiersUiController : NvGenericUiController<PartnerTierEntity>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Include(e => e.PartnerType));
        }

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
                typeof(PartnerTierEntity).GetProperty(nameof(PartnerTierEntity.Name)),
                typeof(PartnerTierEntity).GetProperty(nameof(PartnerTierEntity.LogoSize)),
                typeof(PartnerTierEntity).GetProperty(nameof(PartnerTierEntity.PartnerType))
            };
        }

        protected override void SetTopLinks()
        {
            TopLinks.Add(new AdminDashboardLink("Partners", typeof(PartnersUiController)));
            TopLinks.Add(new AdminDashboardLink("Partner Types", typeof(PartnerTypesUiController)));
        }
    }
}