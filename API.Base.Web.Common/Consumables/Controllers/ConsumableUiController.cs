using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Common.Consumables.Models;
using API.Base.Web.Common.Controllers.Ui.Nv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.Base.Web.Common.Consumables.Controllers
{
    public class ConsumableUiController : NvGenericUiController<ConsumableEntity>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(queryable => queryable
                .OrderBy(p => p.Created)
            );
        }

        public override Task<IActionResult> Index()
        {
            Repo.ChainQueryable(q => q.Include(c => c.ConsumedRecords));
            return base.Index();
        }

        [Authorize(Roles = "Admin")]
        public override Task<IActionResult> Edit(string id)
        {
            return base.Edit(id);
        }

        [Authorize(Roles = "Admin")]
        public override Task<IActionResult> Edit(string id, ConsumableEntity entity)
        {
            return base.Edit(id, entity);
        }

        [Authorize(Roles = "Admin")]
        public override Task<IActionResult> Delete(string id)
        {
            return base.Delete(id);
        }

        [Authorize(Roles = "Admin")]
        public override Task<IActionResult> DeleteConfirmed(string id)
        {
            return base.DeleteConfirmed(id);
        }

        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Scan(string id)
        {
            var entity = await GetOne(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        protected IRepository<User> Users => ServiceProvider.GetService<IDataLayer>().Repo<User>();

        protected IRepository<ConsumedRecordEntity> RepoM2M =>
            ServiceProvider.GetService<IDataLayer>().Repo<ConsumedRecordEntity>();

        public async Task<int> ConsumedCount(ConsumableEntity entity)
        {
            return await RepoM2M.FindAll(cr => cr.Consumable == entity).ToAsyncEnumerable().Count();
        }

        [Authorize(Roles = "Moderator")]
        [HttpGet]
        public async Task<IActionResult> Consume(string id, [FromQuery] [Required] string userCode,
            [FromQuery] bool commit = false)
        {
            ViewBag.Ok = false;
            ViewBag.Saved = false;

            var entity = await GetOne(id);
            if (entity == null)
            {
                return NotFound();
            }

            var user = await Users.GetOne(API.Base.Web.Base.Auth.Models.Entities.User.ParseShortGuid(userCode));
            ViewBag.User = user;
            ViewBag.Consumable = entity;

            if (user == null)
            {
                return PartialView("ConsumeResult");
            }

            var consumed = await RepoM2M.Queryable.CountAsync(e => e.User == user && e.Consumable == entity);
            if (consumed >= entity.Count)
            {
                return PartialView("ConsumeResult");
            }

            ViewBag.Ok = true;
            if (commit)
            {
                await RepoM2M.Add(new ConsumedRecordEntity {User = user, Consumable = entity});
                ViewBag.Saved = true;
                return View("Scan", entity);
            }
            else
            {
                return PartialView("ConsumeResult");
            }
        }
    }
}
