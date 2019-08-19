using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Database.Repository.Helpers;
using API.Base.Web.Base.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace API.Base.Web.Base.Controllers.Ui
{
    public abstract class GenericUiController<TE> : UiController where TE : Entity
    {
        protected virtual IRepository<TE> Repo => ServiceProvider.GetService<IDataLayer>().Repo<TE>();

        protected IDataLayer DataLayer => ServiceProvider.GetService<IDataLayer>();


        public GenericUiController()
        {
        }

        public virtual async Task<IEnumerable<TE>> GetAllEntities()
        {
            return await Repo.GetAll();
        }

        public virtual async Task<IActionResult> Index()
        {
            return View(await GetAllEntities());
        }

        public virtual async Task<IActionResult> ReOrder()
        {
            return View(await GetAllEntities());
        }

        [HttpPost]
        public virtual async Task<IActionResult> SaveOrder(Dictionary<string, int> orderBag)
        {
            var all = await GetAllEntities();
            Repo.SkipSaving = true;
            foreach (var e1 in all)
            {
                var e = (IOrderedEntity) e1;
                if (orderBag.ContainsKey(e.Id))
                {
                    e.OrderIndex = orderBag[e1.Id];
                }
            }

            await DataLayer.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        protected virtual async Task<TE> GetOne(string id)
        {
            return await Repo.GetOne(id);
        }

        public virtual async Task<IActionResult> Details(string id)
        {
            var entity = await GetOne(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        public IActionResult Create()
        {
            return View();
        }


        public virtual async Task<IActionResult> Edit(string id)
        {
            var entity = await GetOne(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }


        public virtual async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await Repo.GetOne(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(string id)
        {
            await Repo.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(string id, TE entity)
        {
            if (id != entity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await Repo.GetOne(id);
                if (existing == null)
                {
                    return NotFound();
                }

                try
                {
                    var epb = EntityUpdateHelper<TE>.GetEpbFor(entity, existing);

                    var updated = await Repo.Patch(epb);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EntityExists(entity.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TE entity)
        {
            if (ModelState.IsValid)
            {
                await SetOrderIndexIfNeeded(entity);
                EntityUpdateHelper<TE>.ClearNullRelatedEntities(entity);
                await Repo.Add(entity);
                return RedirectToAction(nameof(Index));
            }

            return View(entity);
        }

        protected async Task<bool> EntityExists(string id)
        {
            return await Repo.Exists(id);
        }

        protected async Task SetOrderIndexIfNeeded(TE entity)
        {
            if (entity is IOrderedEntity orderedEntity)
            {
                if (orderedEntity.OrderIndex == 0)
                {
                    orderedEntity.OrderIndex =
                        await Repo.Queryable.DefaultIfEmpty().MaxAsync(e => ((IOrderedEntity) e).OrderIndex) + 1;
                }
            }
        }
    }
}