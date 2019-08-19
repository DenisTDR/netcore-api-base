using System;
using API.Base.Web.Base.Models.Entities;
using API.Base.Web.Base.Models.EntityMaps;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void AddConfiguration<TEntity>(this ModelBuilder modelBuilder,
            IEntityTypeConfiguration<TEntity> configuration)
            where TEntity : class, IEntity
        {
            configuration.Configure(modelBuilder.Entity<TEntity>());
        }

        public static void AddConfigurationType(this ModelBuilder modelBuilder, Type configurationType)
        {
            var instance = (ITypeConfiguration) Activator.CreateInstance(configurationType);

            var methodInfo = typeof(ModelBuilderExtensions).GetMethod(nameof(AddConfiguration));
            var genericMethodInfo = methodInfo.MakeGenericMethod(instance.GetEntityType());

            genericMethodInfo.Invoke(null, new object[] {modelBuilder, instance});
        }
    }
}