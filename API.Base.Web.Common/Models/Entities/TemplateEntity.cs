using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Models;
using API.Base.Web.Base.Models.Entities;
using API.Base.Web.Common.Models.ViewModels;

namespace API.Base.Web.Common.Models.Entities
{
    [AutoMapsWith(typeof(TemplateViewModel))]
    public class TemplateEntity : Entity, ISlugableEntity
    {
        [Required] public string Slug { get; set; }

        public string Title { get; set; }

        [DataType(DataType.Html)] [Required] public string Content { get; set; }
        public string Description { get; set; }
        public TemplateType Type { get; set; }

        public override string ToString()
        {
            return Slug;
        }
    }
}