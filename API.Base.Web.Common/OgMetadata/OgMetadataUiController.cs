using API.Base.Web.Common.Controllers.Ui.Nv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Common.OgMetadata
{
    [Authorize(Roles = "Moderator")]
    public class OgMetadataUiController : NvGenericUiController<OgMetadataEntity>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(queryable => queryable.Include(e => e.Image));
        }
    }
}