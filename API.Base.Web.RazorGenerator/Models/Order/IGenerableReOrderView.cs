using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.RazorGenerator.Models.Order
{
    public interface IGenerableOrderView<TE> : IGenerableView<TE> where TE : Entity
    {
    }
}