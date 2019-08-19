using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Models.EntityMaps;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Base.Logging.Models.EntityMaps
{
    public class AuditMap : EntityTypeConfiguration<AuditEntity>
    {
        public override void Configure(EntityTypeBuilder<AuditEntity> builder)
        {
            base.Configure(builder);
            builder.HasIndex(e => e.TraceIdentifier);
        }
    }
}