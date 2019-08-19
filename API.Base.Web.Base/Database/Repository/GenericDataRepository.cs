using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository.Helpers;
using API.Base.Web.Base.Misc;
using API.Base.Web.Base.Misc.PatchBag;
using API.Base.Web.Base.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Database.Repository
{
    public class GenericRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly IDataLayer _dataLayer;
        private readonly DbSet<T> _dbSet;
        private IQueryable<T> _queryable;

        public DbSet<T> DbSet => _dbSet;
        public IQueryable<T> Queryable => _queryable;

        public virtual async Task<IEntity> GetOneEntity(string selector)
        {
            return await GetOne(selector);
        }

        public virtual void RebuildQueryable(Func<IQueryable<T>, IQueryable<T>> func)
        {
            _queryable = func(_dbSet);
        }

        public void ChainQueryable(Func<IQueryable<T>, IQueryable<T>> func)
        {
            _queryable = func(_queryable);
        }

        public bool SkipSaving { get; set; } = false;

        public GenericRepository(DbSet<T> dbSet, IDataLayer dataLayer)
        {
            _dbSet = dbSet;
            _queryable = dbSet.Where(e => !e.Deleted);
            _dataLayer = dataLayer;
            if (typeof(IOrderedEntity).IsAssignableFrom(typeof(T)))
            {
                _queryable = _queryable.OrderBy(e => ((IOrderedEntity) e).OrderIndex);
            }
        }

        public virtual async Task<T> GetOne(string selectorOrId, bool includeDeleted = false)
        {
            if (selectorOrId == null)
            {
                return null;
            }

            var query = _queryable.Where(e => e.Selector == selectorOrId || e.Id == selectorOrId);

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAll(bool includeDeleted = false, bool dontFetch = false)
        {
            var query = _queryable;
            if (!dontFetch)
            {
                return await query.ToListAsync();
            }

            return query;
        }

        public virtual async Task<T> Add(T e)
        {
//            Console.WriteLine("adding a " + typeof(T).Name);
            e.Selector = Utilis.GenerateSelector();
            e.Created = e.Updated = DateTime.Now;
            e.Deleted = false;
            await new EntityUpdateHelper<T>(_dataLayer, _dbSet).BindRelatedEntities(e);
            var addingResult = await _dbSet.AddAsync(e);

            if (!SkipSaving)
            {
                await _dataLayer.SaveChangesAsync();
            }

//            Console.WriteLine("added a " + typeof(T).Name);
            return addingResult.Entity;
        }

        public virtual async Task<T> AddOrUpdate(T e)
        {
            if (!Utilis.IsSelector(e.Selector))
            {
                return await this.Add(e);
            }

            var existing = _dbSet.FirstOrDefaultAsync(entity => entity.Selector == e.Selector);
            if (existing == null)
            {
                return await this.Add(e);
            }

            return await this.Update(e);
        }

        public virtual async Task<T> Update(T e)
        {
            e.Updated = DateTime.Now;
            _dbSet.Update(e);
            if (!SkipSaving)
            {
                await _dataLayer.SaveChangesAsync();
            }

            return e;
        }

        public virtual async Task<T> Patch(EntityPatchBag<T> eub)
        {
            var existing = await GetOne(eub.Id);
            if (!eub.HasAnything)
            {
                return existing;
            }

            await new EntityUpdateHelper<T>(_dataLayer, _dbSet).TakeCareOf(eub, existing);
            existing.Updated = DateTime.Now;

            if (!SkipSaving)
            {
                await _dataLayer.SaveChangesAsync();
            }

            return existing;
        }

        public virtual async Task<bool> Exists(string selector, bool includeDeleted = false)
        {
            IQueryable<T> query = _queryable;
            if (!includeDeleted)
            {
                query = query.Where(e => !e.Deleted);
            }

            return await query.AnyAsync(e => e.Selector == selector);
        }

        public virtual async Task<bool> Delete(string idOrSelector)
        {
            //            Console.WriteLine("Repo Delete: " + selector);
            var existing = await GetOne(idOrSelector);
            if (existing == null)
            {
                return false;
            }

            existing.Deleted = true;
//            DbSet.Update(existing);
            if (!SkipSaving)
            {
                await _dataLayer.SaveChangesAsync();
            }

            return true;
        }

        public virtual async Task<T> FindOne(Expression<Func<T, bool>> predicate)
        {
            return await _queryable.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await _queryable.ToListAsync();
            }

            return await _queryable.Where(predicate).ToListAsync();
        }

        public virtual async Task<bool> Any(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await _queryable.CountAsync() > 0;
            }

            return await _queryable.AnyAsync(predicate);
        }
    }
}