using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Models.EntityMaps;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Base.Logging.Models.EntityMaps
{
    public class AuditMap : EntityTypeConfiguration<LogsAuditEntity>
    {
        public override void Configure(EntityTypeBuilder<LogsAuditEntity> builder)
        {
            base.Configure(builder);
            builder.HasIndex(e => e.TraceIdentifier);
        }
    }
}