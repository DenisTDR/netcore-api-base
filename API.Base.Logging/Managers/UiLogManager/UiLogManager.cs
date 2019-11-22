using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Database.DataLayer;

namespace API.Base.Logging.Managers.UiLogManager
{
    internal class UiLogManager : DbStoredLogManager<LogsUiEntity>, IUiLogManager
    {
        internal UiLogManager(IDataLayer dataLayer) : base(dataLayer)
        {
        }
    }
}