using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class TimeZoneMapping : IEntityTypeConfiguration<DbTimeZone>
    {
        public void Configure(EntityTypeBuilder<DbTimeZone> builder)
        {
            builder.ToTable("TimeZones");
            builder.HasKey(t => t.OffsetMinutes).IsClustered(false);
            builder.Property(t => t.OffsetMinutes).ValueGeneratedNever();
            builder.Property(t => t.Name).IsRequired().HasMaxLength(255);
        }
    }
}