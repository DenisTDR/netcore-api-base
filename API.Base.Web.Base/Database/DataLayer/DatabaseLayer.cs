using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Exceptions;
using API.Base.Web.Base.Misc;
using API.Base.Web.Base.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Database.DataLayer
{
    public class DatabaseLayer : IDataLayer
    {
        private readonly BaseDbContext _dbContext;
        private readonly IDictionary<Type, IDataRepository> _reposCache = new Dictionary<Type, IDataRepository>();

        public DatabaseLayer(BaseDbContext dbContext)
        {
//            Console.WriteLine("DatabaseLayer ctor");
            _dbContext = dbContext;
        }

        public IRepository<T> Repo<T>() where T : class, IEntity
        {
            var type = typeof(T);
            if (!_reposCache.ContainsKey(type))
            {
                var dbSet = _dbContext.Set<T>();
                var repo = new GenericRepository<T>(dbSet, this);
                _reposCache.Add(type, repo);
            }

            return (IRepository<T>) _reposCache[type];
        }

        public IDataRepository Repository(Type entityType)
        {
            var methodInfo = this.GetType().GetMethod(nameof(Repo));
            methodInfo = methodInfo.MakeGenericMethod(entityType);

            return (IDataRepository) methodInfo.Invoke(this, new object[] { });
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task MigrateDatabase()
        {
            await _dbContext.Database.MigrateAsync();
        }

        public async Task EnsureMigrated()
        {
            try
            {
                if ((await _dbContext.Database.GetPendingMigrationsAsync()).Any())
                {
                    Utilis.DieWith("There are pending migrations!\nrun 'dotnet run --migrate true'");
                }
            }
            catch (Exception e)
            {
                Utilis.DieWith("Can't ensure database is migrated. \nReason: " + e.Message);
            }
        }

        public void SetRepo<T>(IRepository<T> repo) where T : class, IEntity
        {
            var type = typeof(T);
            if (_reposCache.ContainsKey(type))
            {
                throw new KnownException("Repository for '" + type.Name + "' already exists.");
            }

            _reposCache.Add(type, repo);
        }
    }
}