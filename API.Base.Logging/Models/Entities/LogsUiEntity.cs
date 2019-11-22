using API.Base.Web.Base.Auth.Models.Entities;

namespace API.Base.Logging.Models.Entities
{
    public class LogsUiEntity : DbStoredLog
    {
        public User Author { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string TargetId { get; set; }
        public string OldVersion { get; set; }
        public string NewVersion { get; set; }
        public string Url { get; set; }
    }
}