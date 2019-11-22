using API.Base.Web.Base.Data;
using API.Base.Web.Base.Models.ViewModels;

namespace API.Base.Files.Models.ViewModels
{
    public class FileViewModel : ViewModel, IFile
    {
        public string Link { get; set; }
        public bool Protected { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}