using System;
using System.Threading.Tasks;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Database.DataLayer
{
    public interface IDataLayer
    {
        IRepository<T> Repo<T>() where T : class, IEntity;

        IDataRepository Repository(Type entityType);

        //
        Task SaveChangesAsync();
        //        void Untrack(Entity e);
        //        Task<IDbContextTransaction> BeginTransaction();
        //        DbContext DbContext { get; }

        Task MigrateDatabase();
        Task EnsureMigrated();

        void SetRepo<T>(IRepository<T> repo) where T : class, IEntity;
    }
}