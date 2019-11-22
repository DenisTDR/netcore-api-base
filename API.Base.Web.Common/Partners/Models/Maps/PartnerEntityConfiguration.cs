using API.Base.Web.Base.Models.EntityMaps;
using API.Base.Web.Common.Partners.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Base.Web.Common.Partners.Models.Maps
{
    public class PartnerEntityConfiguration : EntityTypeConfiguration<PartnerEntity>
    {
        public override void Configure(EntityTypeBuilder<PartnerEntity> builder)
        {
            base.Configure(builder);
            builder.HasOne(p => p.Type).WithMany(t => t.Partners);
            builder.HasOne(p => p.Tier).WithMany(t => t.Partners);
        }
    }
}