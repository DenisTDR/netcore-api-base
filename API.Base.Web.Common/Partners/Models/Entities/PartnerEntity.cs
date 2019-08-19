using System.ComponentModel.DataAnnotations;
using API.Base.Files.Models.Entities;
using API.Base.Web.Base.Attributes;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Common.Partners.Models.Entities
{
    public class PartnerEntity : Entity, IPublishableEntity, IOrderedEntity
    {
        public string Name { get; set; }
        public FileEntity Logo { get; set; }
        public PartnerTypeEntity Type { get; set; }
        public PartnerTierEntity Tier { get; set; }
        [DataType(DataType.Url)] public string Url { get; set; }
        public bool Published { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        [IsReadOnly] public int OrderIndex { get; set; }
    }
}