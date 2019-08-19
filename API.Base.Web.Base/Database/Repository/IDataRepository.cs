using System.Threading.Tasks;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Base.Database.Repository
{
    public interface IDataRepository
    {
        Task<IEntity> GetOneEntity(string selector);
        bool SkipSaving { get; set; }
    }
}