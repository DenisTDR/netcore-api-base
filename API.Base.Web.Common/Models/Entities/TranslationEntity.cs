using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Common.Models.Entities
{
    public class TranslationEntity : Entity, ISlugableEntity
    {
        [Required] public string Slug { get; set; }
        [DataType(DataType.MultilineText)] public string Value { get; set; }
        [Required] public string Language { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Slug;
        }
    }
}