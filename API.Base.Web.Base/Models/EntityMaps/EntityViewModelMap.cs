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
        public virtual void ConfigureEntityToViewModelMapper(IMapperConfigurationExpression configurationExpression)
        {
            BasicConfigureEntityToViewModelMapper(configurationExpression.CreateMap<TEntity, TViewModel>());
        }

        public virtual void ConfigureViewModelToEntityMapper(IMapperConfigurationExpression configurationExpression)
        {
            BasicConfigureViewModelToEntityMapper(configurationExpression.CreateMap<TViewModel, TEntity>());
        }


        protected void BasicConfigureEntityToViewModelMapper(IMappingExpression<TEntity, TViewModel> expression)
        {
            expression.AfterMap((entity, viewModel) => { viewModel.Id = entity.Selector; }).PreserveReferences();
        }

        protected void BasicConfigureViewModelToEntityMapper(IMappingExpression<TViewModel, TEntity> expression)
        {
            expression.ForMember(te => te.Id, opt => opt.Ignore())
                .AfterMap((viewModel, entity) => { entity.Selector = viewModel.Id; });
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