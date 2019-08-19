using API.Base.Web.Base.Database;
using API.Base.Web.Base.Database.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Factories
{
    public static class DataLayerFactory
    {
        public static IDataLayer BuildDataLayer()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();
            ApiBuilder.ApiBuilder.Instance.PutMysql(dbContextOptionsBuilder);
            var dbContextOptions = dbContextOptionsBuilder.Options;
            var dbContext = new BaseDbContext(dbContextOptions);
            var dataLayer = new DatabaseLayer(dbContext);
            return dataLayer;
        }
    }
}