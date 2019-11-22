using System.Threading.Tasks;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Base.Database.Repository
{
    public interface IDataRepository
    {
        Task<IEntity> GetOneEntity(string id);
        bool SkipSaving { get; set; }
    }
}