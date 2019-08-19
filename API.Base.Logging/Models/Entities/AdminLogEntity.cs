using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Logging.Models.Entities
{
    public class AdminLogEntity : Entity
    {
        public User Author { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string TargetId { get; set; }
        public string OldVersion { get; set; }
        public string NewVersion { get; set; }
        public string Url { get; set; }
        public string TraceIdentifier { get; set; }
    }
}