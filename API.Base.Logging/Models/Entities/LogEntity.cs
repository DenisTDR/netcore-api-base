using System.ComponentModel.DataAnnotations.Schema;

namespace API.Base.Logging.Models.Entities
{
    [NotMapped]
    public class LogEntity : LogBaseEntity
    {
        public string Message { get; set; }
    }
}