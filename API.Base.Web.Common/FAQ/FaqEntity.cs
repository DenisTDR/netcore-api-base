using System.ComponentModel.DataAnnotations;
using API.Base.Web.Base.Attributes;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Common.FAQ
{
    public class FaqEntity : Entity, IPublishableEntity, IOrderedEntity
    {
        public string Question { get; set; }


        [DataType(DataType.Html)] public string Answer { get; set; }

        [IsReadOnly] public int OrderIndex { get; set; }

        [MaxLength(256)] public string Category { get; set; }
        public bool Published { get; set; }

        public override string ToString()
        {
            return Question;
        }
    }
}