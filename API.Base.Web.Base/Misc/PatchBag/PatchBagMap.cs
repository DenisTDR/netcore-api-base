using System;
using API.Base.Web.Base.Models.EntityMaps;
using AutoMapper;

namespace API.Base.Web.Base.Misc.PatchBag
{
    internal class PatchBagMap: IEntityViewModelMap
    {
        public void ConfigureEntityToViewModelMapper(IMapperConfigurationExpression configurationExpression)
        {
            configurationExpression.CreateMap(GetEntityType(), GetViewModelType());
        }

        public void ConfigureViewModelToEntityMapper(IMapperConfigurationExpression configurationExpression)
        {
            configurationExpression.CreateMap(GetViewModelType(), GetEntityType());
        }

        public Type GetEntityType()
        {
            return typeof(EntityPatchBag<>);
        }

        public Type GetViewModelType()
        {
            return typeof(ViewModelPatchBag<>);
        }
    }
}
