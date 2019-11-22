using Microsoft.Extensions.Logging;

namespace API.Base.Logging.Models.Entities
{
    public interface ILog
    {
        LogLevel Level { get; set; }
        string Tag { get; set; }
    }
}