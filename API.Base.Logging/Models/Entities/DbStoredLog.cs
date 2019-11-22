using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Logging.Models.Entities
{
    public abstract class DbStoredLog : Entity
    {
        [MaxLength(128)] public string TraceIdentifier { get; set; }
    }
}