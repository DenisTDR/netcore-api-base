using API.Base.Web.Base.Models.Entities;
using Microsoft.Extensions.Logging;

namespace API.Base.Logging.Models.Entities
{
    public abstract class LogBaseEntity : Entity
    {
        public LogLevel Level { get; set; }
        public string Tag { get; set; }
    }
}