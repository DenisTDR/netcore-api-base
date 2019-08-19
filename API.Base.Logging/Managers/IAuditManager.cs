using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Misc.PatchBag;

namespace API.Base.Logging.Managers
{
    public interface IAuditManager
    {
        void Store(AuditEntity audit);
        void UpdateAuditLog(EntityPatchBag<AuditEntity> epbae);
    }
}