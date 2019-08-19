using System.Threading.Tasks;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Base.Misc.PatchBag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Base.Web.Common.ReferenceTrack
{
    public class ReferenceTrackController : GenericCrudController<ReferenceTrackEntity, ReferenceTrackViewModel>
    {
        [AllowAnonymous]
        public override Task<IActionResult> Add(ReferenceTrackViewModel vm)
        {
            return base.Add(vm);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<IActionResult> Delete(string id)
        {
            return new Task<IActionResult>(Ok);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<IActionResult> Patch(ViewModelPatchBag<ReferenceTrackViewModel> vmub)
        {
            return new Task<IActionResult>(Ok);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<IActionResult> GetAll()
        {
            return new Task<IActionResult>(Ok);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<IActionResult> GetOne(string id)
        {
            return new Task<IActionResult>(Ok);
        }
    }
}