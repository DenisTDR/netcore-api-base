using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Database.Factories
{
    public interface IDbContextFactory
    {
        DbContext Create();
    }
}
