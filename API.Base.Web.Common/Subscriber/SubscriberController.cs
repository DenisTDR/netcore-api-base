using System.Threading.Tasks;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Base.Misc.PatchBag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Base.Web.Common.Subscriber
{
    public class SubscriberController : GenericCrudController<SubscriberEntity, SubscriberViewModel>
    {
        [AllowAnonymous]
        public override async Task<IActionResult> Add(SubscriberViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing =
                await Repo.FindOne(e => vm.Email.Equals(e.Email));
            if (existing != null)
            {
                var existingVm = Map(existing);
                return Ok(existingVm);
            }

            return await base.Add(vm);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<IActionResult> Delete(string id)
        {
            return base.Delete(id);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<IActionResult> Patch(ViewModelPatchBag<SubscriberViewModel> vmub)
        {
            return base.Patch(vmub);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<IActionResult> GetAll()
        {
            return base.GetAll();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override Task<IActionResult> GetOne(string id)
        {
            return base.GetOne(id);
        }
    }
}