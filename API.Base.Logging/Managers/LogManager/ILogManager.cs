using API.Base.Logging.Models.Entities;

namespace API.Base.Logging.Managers.LogManager
{
    public interface ILogManager
    {
        void StoreAsync(LogEntity log);
    }
}