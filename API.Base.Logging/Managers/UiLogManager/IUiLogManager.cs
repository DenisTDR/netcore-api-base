using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Misc.PatchBag;

namespace API.Base.Logging.Managers.UiLogManager
{
    public interface IUiLogManager
    {
        void Store(LogsUiEntity audit);
        void UpdateStoredLog(EntityPatchBag<LogsUiEntity> epbae);
    }
}