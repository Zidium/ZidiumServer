using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class EventTypeMapping : IEntityTypeConfiguration<DbEventType>
    {
        public void Configure(EntityTypeBuilder<DbEventType> builder)
        {
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.ToTable("EventTypes");
            builder.Property(t => t.DisplayName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Code).HasMaxLength(20);
            builder.Property(t => t.OldVersion).HasMaxLength(255);
            builder.HasIndex(t => t.SystemName);
        }
    }
}
