using System;
using API.Base.Web.Base.Models.Entities;
using API.Base.Web.Base.Models.ViewModels;
using AutoMapper;

namespace API.Base.Web.Base.Models.EntityMaps
{
    public class EntityViewModelMap<TEntity, TViewModel> : IEntityViewModelMap
        where TEntity : class, IEntity
        where TViewModel : ViewModel
    {

        protected IMappingExpression<TEntity, TViewModel> EntityToViewModelExpression;
        protected IMappingExpression<TViewModel, TEntity> ViewModelToEntityExpression;
        public virtual void ConfigureEntityToViewModelMapper(IMapperConfigurationExpression configurationExpression)
        {
            EntityToViewModelExpression = configurationExpression.CreateMap<TEntity, TViewModel>();
            EntityToViewModelExpression.PreserveReferences();
        }

        public virtual void ConfigureViewModelToEntityMapper(IMapperConfigurationExpression configurationExpression)
        {
            ViewModelToEntityExpression = configurationExpression.CreateMap<TViewModel, TEntity>();
        }

        public Type GetViewModelType()
        {
            return typeof(TViewModel);
        }

        public Type GetEntityType()
        {
            return typeof(TEntity);
        }
    }
}