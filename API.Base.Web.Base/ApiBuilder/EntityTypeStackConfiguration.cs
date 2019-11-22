using System;
using API.Base.Web.Base.Misc;

namespace API.Base.Web.Base.ApiBuilder
{
    public class EntityTypeStackConfiguration
    {
        public EntityTypeStackConfiguration()
        {
        }

        public EntityTypeStackConfiguration(Type entityType)
        {
            EntityType = entityType;
        }

        public Type EntityType { get; set; }
        public ViewModelMapPairSet ViewModelPairTypes { get; } = new ViewModelMapPairSet();
        public Type EntityTypeConfigurationType { get; set; }

        public bool IsStored => !Disabled && EntityTypeConfigurationType != null;
        public bool Disabled { get; set; }

        public Type ApiControllerType { get; set; }
        public Type UiControllerType { get; set; }
    }
}