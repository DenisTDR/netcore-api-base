using API.Base.Files.Models.Entities;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Common.OgMetadata
{
    public class OgMetadataEntity : Entity, ISlugableEntity
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public OgMetadataType Type { get; set; }
        public FileEntity Image { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}