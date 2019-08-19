using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.RazorGenerator.Models.Edit
{
    public interface IGenerableEditView<TE> : IGenerableView<TE> where TE : Entity
    {
    }
}