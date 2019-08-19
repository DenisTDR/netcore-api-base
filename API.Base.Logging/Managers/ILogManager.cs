using API.Base.Logging.Models.Entities;

namespace API.Base.Logging.Managers
{
    public interface ILogManager
    {
        void StoreAsync(LogEntity log);
    }
}