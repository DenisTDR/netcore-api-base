using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Models.Entities;
using API.Base.Web.Base.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Base.Controllers.Api
{
    public abstract class GenericReadOnlyController<TE, TVm> : ApiController where TE : Entity where TVm : ViewModel
    {
        protected IRepository<TE> Repo => ServiceProvider.GetService<IDataLayer>().Repo<TE>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (typeof(IOrderedEntity).IsAssignableFrom(typeof(TE)))
            {
                Repo.ChainQueryable(q => q.OrderBy(e => ((IOrderedEntity) e).OrderIndex));
            }

            if (typeof(IPublishableEntity).IsAssignableFrom(typeof(TE)))
            {
                Repo.ChainQueryable(q => q.Where(e => ((IPublishableEntity) e).Published));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ViewModel>), (int) HttpStatusCode.OK)]
        public virtual async Task<IActionResult> GetAll()
        {
            var allE = await Repo.GetAll();

            return Ok(Map(allE));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ViewModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public virtual async Task<IActionResult> GetOne(string id)
        {
            var e = await Repo.GetOne(id);

            if (e == null)
            {
                return NotFound();
            }

            return Ok(Map(e));
        }

        [HttpPost]
        public IActionResult IgnoreThisEndpoint([FromBody] TVm viewModel)
        {
            return Ok();
        }

        protected TVm Map(TE e)
        {
            return Mapper.Map<TVm>(e);
        }

        protected IEnumerable<TVm> Map(IEnumerable<TE> es)
        {
            Console.WriteLine("Map: " + typeof(TVm).Name + " from " + typeof(TE).Name);
            return Mapper.Map<IEnumerable<TVm>>(es);
        }
    }
}