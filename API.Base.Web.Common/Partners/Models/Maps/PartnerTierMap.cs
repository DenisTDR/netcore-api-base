using API.Base.Web.Base.Models.EntityMaps;
using API.Base.Web.Common.Partners.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Base.Web.Common.Partners.Models.Maps
{
    public class PartnerTierMap : EntityTypeConfiguration<PartnerTierEntity>
    {
        public override void Configure(EntityTypeBuilder<PartnerTierEntity> builder)
        {
            base.Configure(builder);
            builder.HasOne(pt => pt.PartnerType).WithMany(pt => pt.Tiers);
        }
    }
}