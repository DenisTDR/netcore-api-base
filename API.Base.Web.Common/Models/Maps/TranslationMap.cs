using API.Base.Web.Base.Models.EntityMaps;
using API.Base.Web.Common.Models.Entities;
using API.Base.Web.Common.Models.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Base.Web.Common.Models.Maps
{
    public class TranslationMap : EntityViewModelMap<TranslationEntity, TranslationViewModel>
    {
        public override void ConfigureEntityToViewModelMapper(IMapperConfigurationExpression configurationExpression)
        {
            base.ConfigureEntityToViewModelMapper(configurationExpression);
            EntityToViewModelExpression
                .ForMember(t => t.Id, opt => opt.Ignore());
        }
    }

    public class TranslationEntityConfiguration : EntityTypeConfiguration<TranslationEntity>
    {
        public override void Configure(EntityTypeBuilder<TranslationEntity> builder)
        {
            base.Configure(builder);
            builder.HasIndex(t => t.Language);
        }
    }
}