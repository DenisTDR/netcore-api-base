using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using API.Base.Web.Base.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Base.Web.Base.Models.EntityMaps
{
    public class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>, ITypeConfiguration
        where TEntity : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ToTable(GetTableName());

            builder.HasIndex(e => e.Id);
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Selector);
            builder.Property(e => e.Selector)
                .HasMaxLength(32);

            if (typeof(IOrderedEntity).IsAssignableFrom(typeof(TEntity)))
            {
                builder.HasIndex(e => ((IOrderedEntity) e).OrderIndex);
            }
        }

        public Type GetEntityType()
        {
            return typeof(TEntity);
        }

        public virtual string GetTableName()
        {
            if (typeof(TEntity).GetCustomAttributes(true).LastOrDefault(attr => attr is TableAttribute) is
                TableAttribute toTableAttribute)
            {
                return toTableAttribute.Name;
            }

            var entityName = typeof(TEntity).Name;
            if (entityName.EndsWith("Entity"))
            {
                entityName = entityName.Substring(0, entityName.Length - 6);
            }

            if (entityName.EndsWith("Model"))
            {
                entityName = entityName.Substring(0, entityName.Length - 5);
            }

            return entityName;
        }
    }
}