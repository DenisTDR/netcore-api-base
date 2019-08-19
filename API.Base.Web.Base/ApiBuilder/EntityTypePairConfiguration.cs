using System;
using API.Base.Web.Base.Misc;

namespace API.Base.Web.Base.ApiBuilder
{
    public class EntityTypePairConfiguration
    {
        public EntityTypePairConfiguration()
        {
        }

        public EntityTypePairConfiguration(Type entityType)
        {
            EntityType = entityType;
        }

        public Type EntityType { get; set; }
        public ViewModelMapPairSet ViewModelPairTypes { get; } = new ViewModelMapPairSet();
        public Type EntityTypeConfigurationType { get; set; }

        public bool IsStored => EntityTypeConfigurationType != null;
    }
}