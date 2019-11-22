using System.ComponentModel.DataAnnotations.Schema;
using API.Base.Web.Base.Models.Entities;
using Microsoft.Extensions.Logging;

namespace API.Base.Logging.Models.Entities
{
    [NotMapped]
    public class LogEntity : Entity, ILog
    {
        public LogLevel Level { get; set; }
        public string Tag { get; set; }
        public string Message { get; set; }
    }
}