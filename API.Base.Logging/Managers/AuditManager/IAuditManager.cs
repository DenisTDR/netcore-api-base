using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Misc.PatchBag;

namespace API.Base.Logging.Managers.AuditManager
{
    public interface IAuditManager
    {
        void Store(LogsAuditEntity logsAudit);
        void UpdateStoredLog(EntityPatchBag<LogsAuditEntity> epbae);
    }
}