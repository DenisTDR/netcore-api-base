using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Attributes;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Common.Partners.Models.Entities
{
    public abstract class PartnerGroupEntity : Entity, IOrderedEntity
    {
        public string Name { get; set; }

        [Range(0, 5)] public int LogoSize { get; set; }
        [IsReadOnly] public int OrderIndex { get; set; }
        public virtual List<PartnerEntity> Partners { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}