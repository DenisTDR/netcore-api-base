using API.Base.Logging.Models.Entities;
using API.Base.Web.Base.Models.EntityMaps;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Base.Logging.Models.EntityMaps
{
    public class AdminLogMap : EntityTypeConfiguration<LogsUiEntity>
    {
        public override void Configure(EntityTypeBuilder<LogsUiEntity> builder)
        {
            base.Configure(builder);
            builder.HasIndex(ale => ale.TraceIdentifier);
        }
    }
}