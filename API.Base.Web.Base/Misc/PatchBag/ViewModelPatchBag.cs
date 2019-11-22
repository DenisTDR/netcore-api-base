using System.Collections.Generic;
using API.Base.Web.Base.Models.ViewModels;
using Newtonsoft.Json;

namespace API.Base.Web.Base.Misc.PatchBag
{
    public class ViewModelPatchBag<T> where T : ViewModel
    {
        public string Id { get; set; }
        public T Model { get; set; }

        public Dictionary<string, bool> PropertiesToUpdate { get; set; }

        [JsonIgnore] public Dictionary<string, bool> Props => PropertiesToUpdate;
    }
}
