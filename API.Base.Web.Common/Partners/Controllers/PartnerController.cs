using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Controllers.Api;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Common.Partners.Models.Entities;
using API.Base.Web.Common.Partners.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Common.Partners.Controllers
{
    [AllowAnonymous]
    public class PartnerController : GenericReadOnlyController<PartnerEntity, PartnerViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(e => e.Logo)
                .Include(e => e.Type)
                .Include(e => e.Tier));
        }

        [HttpGet]
//        [ProducesResponseType(typeof(IList<PartnerTypeEntity>), 200)]
        public async Task<IActionResult> GetByTypes()
        {
            var all = await Repo.GetAll();
            var pTypeRepo = ServiceProvider.GetService<IDataLayer>().Repo<PartnerTypeEntity>();
            pTypeRepo.ChainQueryable(q => q
                .Include(e => e.Partners)
                .Include(e => e.Tiers)
                .ThenInclude(e => e.Partners));
            var types = await ServiceProvider.GetService<IDataLayer>().Repo<PartnerTypeEntity>().GetAll();
            foreach (var partnerTypeEntity in types)
            {
                partnerTypeEntity.Partners = partnerTypeEntity.Partners.Where(p => p.Published && !p.Deleted).ToList();
                partnerTypeEntity.Tiers = partnerTypeEntity.Tiers.Where(p => !p.Deleted).ToList();
                foreach (var partnerTierEntity in partnerTypeEntity.Tiers)
                {
                    partnerTierEntity.Partners =
                        partnerTierEntity.Partners.Where(p => p.Published && !p.Deleted).ToList();
                }
            }

            var vmTypes = Mapper.Map<IList<PartnerTypeViewModel>>(types);
//            Console.WriteLine(JsonConvert.SerializeObject(types));
            return Ok(vmTypes);
        }
    }
}