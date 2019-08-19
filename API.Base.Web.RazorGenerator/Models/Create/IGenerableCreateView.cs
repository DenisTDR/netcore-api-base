using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.RazorGenerator.Models.Create
{
    public interface IGenerableCreateView<TE> : IGenerableView<TE> where TE : Entity
    {
        
    }
}