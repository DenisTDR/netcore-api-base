using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Files.Models.Entities
{
    public class FileEntity : Entity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string SubDirectory { get; set; }
        public string Extension { get; set; }
        public string OriginalName { get; set; }
        public int Size { get; set; }
        public bool Protected { get; set; }
        [DataType(DataType.MultilineText)] public string Description { get; set; }

        [NotMapped] public string Link { get; set; }
        [NotMapped] public bool IsImage { get; set; }
    }
}