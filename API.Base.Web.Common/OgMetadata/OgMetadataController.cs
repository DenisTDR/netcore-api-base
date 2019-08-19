using System.Threading.Tasks;
using API.Base.Web.Base.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Common.OgMetadata
{
    public class OgMetadataController : GenericReadOnlyController<OgMetadataEntity, OgMetadataViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Include(e => e.Image));
        }

        [AllowAnonymous]
        public override async Task<IActionResult> GetOne(string id)
        {
            var e = await Repo.GetOne(id);

            if (e == null)
            {
                e = await Repo.FindOne(x => x.Slug == id);
                if (e == null)
                {
                    return NotFound();
                }
            }

            return Ok(Map(e));
        }

        [AllowAnonymous]
        public override Task<IActionResult> GetAll()
        {
            return base.GetAll();
        }
    }
}