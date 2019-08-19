using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.RazorGenerator.Models.Delete
{
    public interface IGenerableDeleteView<TE> : IGenerableView<TE> where TE : Entity
    {
    }
}