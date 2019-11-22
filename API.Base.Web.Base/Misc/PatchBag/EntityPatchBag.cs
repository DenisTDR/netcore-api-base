using System.Collections.Generic;
using System.Linq;
using API.Base.Web.Base.Models.Entities;
using Newtonsoft.Json;

namespace API.Base.Web.Base.Misc.PatchBag
{
    public class EntityPatchBag<T> where T : IEntity
    {
        public string Id { get; set; }
        public T Model { get; set; }

        public Dictionary<string, bool> PropertiesToUpdate { get; set; }

        [JsonIgnore]
        public bool HasAnything => PropertiesToUpdate != null && PropertiesToUpdate.Any(kvp => kvp.Value);
    }
}