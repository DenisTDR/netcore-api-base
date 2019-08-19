using System.Threading.Tasks;

namespace API.Base.Web.Base.Database
{
    public interface IDataSeeder
    {
        Task MigrateDatabase();
        Task LoadSeed();
        Task SeedToFile(string param);
        Task<string> SeedToString(string param);
        Task EnsureMigrated();
    }
}