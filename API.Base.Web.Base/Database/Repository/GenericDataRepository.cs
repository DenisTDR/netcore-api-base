using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository.Helpers;
using API.Base.Web.Base.Exceptions;
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

        public virtual async Task<IEntity> GetOneEntity(string id)
        {
            return await GetOne(id);
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
            _queryable = dbSet;
            _dataLayer = dataLayer;
            if (typeof(IOrderedEntity).IsAssignableFrom(typeof(T)))
            {
                _queryable = _queryable.OrderBy(e => ((IOrderedEntity) e).OrderIndex);
            }
        }

        public virtual async Task<T> GetOne(string id)
        {
            if (id == null)
            {
                return null;
            }

            var query = _queryable.Where(e => e.Id == id);
            var entity = await query.FirstOrDefaultAsync();

            if (!(entity is null) || !typeof(T).GetInterfaces().Contains(typeof(ISlugableEntity)))
            {
                return entity;
            }

            query = _queryable.Where(e => ((ISlugableEntity) e).Slug == id);
            entity = await query.FirstOrDefaultAsync();

            return entity;
        }

        public virtual async Task<IEnumerable<T>> GetAll(bool dontFetch = false)
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
            e.Created = e.Updated = DateTime.Now;
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
            var existing = _dbSet.FirstOrDefaultAsync(entity => entity.Id == e.Id);
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

        public virtual async Task<bool> Exists(string id)
        {
            IQueryable<T> query = _queryable;

            return await query.AnyAsync(e => e.Id == id);
        }

        public virtual async Task<bool> Delete(string id)
        {
            //            Console.WriteLine("Repo Delete: " + id);
            var existing = await GetOne(id);
            if (existing == null)
            {
                return false;
            }

            _dbSet.Remove(existing);

            if (!SkipSaving)
            {
                try
                {
                    await _dataLayer.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    throw new KnownException(
                        $"Can't delete {typeof(T).Name} with id: '{id}'. It is referenced by another object.", 400, e,
                        $"Can't delete <i>{existing}</i> ({typeof(T).Name} with id: <i>{id}</i>). It is referenced by another object.");
                }
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