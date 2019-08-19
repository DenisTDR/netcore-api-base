using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Models;
using API.Base.Web.Common.Controllers.Ui.Nv;
using API.Base.Web.Common.Partners.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
            TopLinks.Add(new ControllerActionLinkModel("Partners", controller: nameof(PartnersUiController)));
            TopLinks.Add(new ControllerActionLinkModel("Partner Types", controller: nameof(PartnerTypesUiController)));
        }
    }
}