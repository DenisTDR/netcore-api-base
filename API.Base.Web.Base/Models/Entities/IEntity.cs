using API.Base.Web.Base.Attributes;

namespace API.Base.Web.Base.Models.Entities
{
    public interface IEntity : IBaseEntity
    {
        [IsReadOnly] string Selector { get; set; }

        [IsReadOnly] bool Deleted { get; set; }
    }
}