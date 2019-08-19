using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using API.Base.Web.Base.Misc.PatchBag;
using API.Base.Web.Base.Models.Entities;
using API.Base.Web.Base.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Base.Web.Base.Controllers.Api
{
    public abstract class GenericCrudController<TE, TVm> : GenericReadOnlyController<TE, TVm>
        where TE : Entity where TVm : ViewModel
    {
       

        [HttpPost]
        [ProducesResponseType(typeof(ViewModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public virtual async Task<IActionResult> Add([FromBody] TVm vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var e = Mapper.Map<TE>(vm);
            e = await Repo.Add(e);
            vm = Mapper.Map<TVm>(e);
            return Ok(vm);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public virtual async Task<IActionResult> Delete(string id)
        {
            if (await Repo.Delete(id))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPatch]
        [ProducesResponseType(typeof(ViewModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public virtual async Task<IActionResult> Patch([FromBody] ViewModelPatchBag<TVm> vmub)
        {
            var eub = Mapper.Map<EntityPatchBag<TE>>(vmub);

            if (!await Repo.Exists(eub.Id))
            {
                return NotFound();
            }

            var e = await Repo.Patch(eub);

            var vm = Map(e);

            return Ok(vm);
        }


        protected TE Map(TVm vm)
        {
            return Mapper.Map<TE>(vm);
        }

        protected IList<TE> Map(IEnumerable<TVm> vms)
        {
            return Mapper.Map<IList<TE>>(vms);
        }
    }
}