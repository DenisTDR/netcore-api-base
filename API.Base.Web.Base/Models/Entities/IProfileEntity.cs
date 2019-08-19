using API.Base.Web.Base.Auth.Models.Entities;

namespace API.StartApp.Models.Entities
{
    public interface IProfileEntity
    {
        User User { get; set; }
    }
}
