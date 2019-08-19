using API.Base.Web.Base.Attributes;

namespace API.Base.Web.Base.Models.Entities
{
    public class Entity : BaseEntity, IEntity
    {
        [IsReadOnly] public string Selector { get; set; }

        [IsReadOnly] public bool Deleted { get; set; }
    }
}