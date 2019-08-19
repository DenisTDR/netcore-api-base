using System;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.RazorGenerator.Models
{
    public interface IGenerableView
    {
        void SetServiceProvider(IServiceProvider serviceProvider);
        void BuildGenerableMetadata();
    }

    public interface IGenerableView<TE> : IGenerableView where TE : Entity
    {
    }
}