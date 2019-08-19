using System;
using API.Base.Web.Base.Attributes;

namespace API.Base.Web.Base.Models.Entities
{
    public interface IBaseEntity
    {
        [IsReadOnly] string Id { get; set; }

        [IsReadOnly] DateTime Created { get; set; }

        [IsReadOnly] DateTime Updated { get; set; }
    }
}