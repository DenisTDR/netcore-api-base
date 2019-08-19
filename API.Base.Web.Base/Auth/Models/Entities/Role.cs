using System;
using API.Base.Web.Base.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Base.Web.Base.Auth.Models.Entities
{
    public class Role : IdentityRole, IEntity
    {
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Selector { get; set; }
        public bool Deleted { get; set; }
    }
}