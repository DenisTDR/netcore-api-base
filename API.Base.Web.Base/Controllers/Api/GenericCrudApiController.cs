using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using API.Base.Web.Base.Attributes;
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
            if (!await Repo.Exists(vmub.Id))
            {
                return NotFound();
            }

            ClearForbiddenPatchFields(vmub);

            var eub = Mapper.Map<EntityPatchBag<TE>>(vmub);
            var e = await Repo.Patch(eub);
            var vm = Map(e);

            return Ok(vm);
        }

        protected void ClearForbiddenPatchFields<T>(ViewModelPatchBag<T> vmpb) where T : ViewModel
        {
            // find properties that are about to be patched but them cannot be updated (CantPatchPropertyFunc)
            // set them to false so the Repo won't touch them
            foreach (var pn in vmpb.Props.Keys.ToList().Where(pn => vmpb.Props[pn]).Where(pn =>
                CantPatchPropertyFunc(typeof(T).GetProperty(pn,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance))))
            {
                vmpb.Props[pn] = false;
            }
        }

        protected Func<PropertyInfo, bool> CantPatchPropertyFunc = prop =>
        {
            // a property marked as ReadOnly (with IsReadOnlyAttribute) cannot be patched 
            return prop != null && prop.GetCustomAttributes<IsReadOnlyAttribute>().Any(attr => attr.Is);
        };

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