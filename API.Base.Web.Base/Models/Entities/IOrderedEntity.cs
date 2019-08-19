using API.Base.Web.Base.Attributes;

namespace API.Base.Web.Base.Models.Entities
{
    public interface IOrderedEntity : IBaseEntity
    {
        [IsReadOnly] int OrderIndex { get; set; }
    }
}