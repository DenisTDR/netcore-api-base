using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Base.Logging.Models.Entities
{
    public class LogsAuditEntity : DbStoredLog, ILog
    {
        public string Ip { get; set; }

        [NotMapped] public UserBasicLogModel Author { get; set; }

        [MaxLength(1024)]
        public string AuthInfo
        {
            get { return JsonConvert.SerializeObject(Author); }
            set { Author = value == null ? null : JsonConvert.DeserializeObject<UserBasicLogModel>(value); }
        }

        [MaxLength(10240)] public string Data { get; set; }

        [MaxLength(10240)] public string Info { get; set; }

        [MaxLength(10240)] public string RequestBody { get; set; }

        [MaxLength(10240)] public string Result { get; set; }

        [MaxLength(512)] public string RequestUri { get; set; }
        public int ResponseDuration { get; set; }
        public LogLevel Level { get; set; }
        public string Tag { get; set; }
    }
}