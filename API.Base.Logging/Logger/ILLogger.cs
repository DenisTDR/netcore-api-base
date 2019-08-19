using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Misc.PatchBag;
using Microsoft.Extensions.Logging;

namespace API.Base.Logging.Logger
{
    public interface ILLogger
    {
        void Log(LogLevel level, string message);
        void Log(LogBaseEntity log);
        void Log(LogLevel level, LogBaseEntity ligaLog);


        void LogError(string message);
        void LogWarn(string message);
        void LogInfo(string message);

        void UpdateAuditLog(EntityPatchBag<AuditEntity> euhae);
    }
}