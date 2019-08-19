using System;
using API.Base.Web.Base.Attributes;

namespace API.Base.Web.Base.Models.Entities
{
    public class BaseEntity : IBaseEntity
    {
        [IsReadOnly] public string Id { get; set; }

        [IsReadOnly] public DateTime Created { get; set; }

        [IsReadOnly] public DateTime Updated { get; set; }
    }
}