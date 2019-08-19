using System;

namespace API.Base.Web.Base.Models.Entities
{
    public class Model
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public bool Deleted { get; set; }
    }
}
