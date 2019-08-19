using API.Base.Web.Base.Models.Entities;
using API.Base.Web.RazorGenerator.Models.Create;
using API.Base.Web.RazorGenerator.Models.Delete;
using API.Base.Web.RazorGenerator.Models.Details;
using API.Base.Web.RazorGenerator.Models.Display;
using API.Base.Web.RazorGenerator.Models.Edit;
using API.Base.Web.RazorGenerator.Models.Form;
using API.Base.Web.RazorGenerator.Models.Index;
using API.Base.Web.RazorGenerator.Models.Order;

namespace API.Base.Web.RazorGenerator.Models
{
    public interface IGenerableViews<TE> : IGenerableIndexView<TE>, IGenerableDisplayView<TE>,
        IGenerableDetailsView<TE>, IGenerableFormView<TE>, IGenerableCreateView<TE>, IGenerableEditView<TE>,
        IGenerableDeleteView<TE>, IGenerableOrderView<TE>
        where TE : Entity
    {
    }
}