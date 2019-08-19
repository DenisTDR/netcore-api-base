using System.Threading.Tasks;
using API.Base.Web.Base.Models;
using API.Base.Web.Common.Controllers.Ui.Nv;
using API.Base.Web.Common.Partners.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Common.Partners.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class PartnersUiController : NvGenericUiController<PartnerEntity>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(e => e.Logo)
                .Include(e => e.Type)
                .Include(e => e.Tier));
        }

        protected override void SetTopLinks()
        {
            TopLinks.Add(new ControllerActionLinkModel("Partner Tiers", controller: nameof(PartnerTiersUiController)));
            TopLinks.Add(new ControllerActionLinkModel("Partner Types", controller: nameof(PartnerTypesUiController)));
        }
    }
}