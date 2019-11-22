using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Database.DataLayer;

namespace API.Base.Logging.Managers.AuditManager
{
    internal class AuditManager : DbStoredLogManager<LogsAuditEntity>, IAuditManager
    {
        internal AuditManager(IDataLayer dataLayer) : base(dataLayer)
        {
        }
    }
}