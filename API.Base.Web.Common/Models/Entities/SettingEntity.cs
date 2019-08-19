using System.ComponentModel.DataAnnotations;
using API.Base.Files.Models.Entities;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Common.Models.Entities
{
    public class SettingEntity : Entity, ISlugableEntity
    {
        public string Slug { get; set; }
        public SettingType Type { get; set; }
        public FileEntity File { get; set; }
        public string Value { get; set; }
        public string Category { get; set; }
        [MaxLength(256)]
        public string Description { get; set; }
    }
}