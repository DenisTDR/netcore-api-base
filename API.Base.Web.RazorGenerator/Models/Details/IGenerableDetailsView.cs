using System.Collections.Generic;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.RazorGenerator.Models.Details
{
    public interface IGenerableDetailsView<TE> : IGenerableView<TE> where TE : Entity
    {
        IList<string> DetailsActions { get; set; }
    }
}