using API.Base.Web.Base.Attributes;

namespace API.Base.Web.Base.Models.ViewModels
{
    public class ViewModel
    {
        [IsReadOnly]
        public string Id { get; set; }
        [IsReadOnly]
        public bool? Deleted { get; set; }
    }
}
