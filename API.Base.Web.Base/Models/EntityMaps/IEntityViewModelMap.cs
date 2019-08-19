using System;
using AutoMapper;

namespace API.Base.Web.Base.Models.EntityMaps
{
    public interface IEntityViewModelMap
    {
        void ConfigureEntityToViewModelMapper(IMapperConfigurationExpression configurationExpression);
        void ConfigureViewModelToEntityMapper(IMapperConfigurationExpression configurationExpression);

        Type GetEntityType();
        Type GetViewModelType();
    }
}