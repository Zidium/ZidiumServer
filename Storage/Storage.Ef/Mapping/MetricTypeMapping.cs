using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class MetricTypeMapping : IEntityTypeConfiguration<DbMetricType>
    {
        public void Configure(EntityTypeBuilder<DbMetricType> builder)
        {
            builder.ToTable("MetricTypes");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.DisplayName).HasMaxLength(255);
            builder.Property(t => t.ConditionAlarm).HasMaxLength(500);
            builder.Property(t => t.ConditionWarning).HasMaxLength(500);
            builder.Property(t => t.ConditionSuccess).HasMaxLength(500);
        }
    }
}
