using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Base.Web.Base.Misc.PatchBag;
using API.Base.Web.Base.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Database.Repository
{
    public interface IRepository<T> : IDataRepository where T : class, IEntity
    {
        Task<T> GetOne(string selectorOrId, bool includeDeleted = false);
        Task<IEnumerable<T>> GetAll(bool includeDeleted = false, bool dontFetch = false);
        Task<T> Add(T e);
        Task<T> Update(T e);
        Task<T> Patch(EntityPatchBag<T> eub);
        Task<T> AddOrUpdate(T e);
        Task<T> FindOne(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate = null);
        Task<bool> Any(Expression<Func<T, bool>> predicate = null);
        Task<bool> Exists(string selector, bool includeDeleted = false);

        Task<bool> Delete(string idOrSelector);

        DbSet<T> DbSet { get; }
        IQueryable<T> Queryable { get; }
        void RebuildQueryable(Func<IQueryable<T>, IQueryable<T>> func);
        void ChainQueryable(Func<IQueryable<T>, IQueryable<T>> func);
    }
}