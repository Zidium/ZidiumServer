using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class MetricMapping : IEntityTypeConfiguration<DbMetric>
    {
        public void Configure(EntityTypeBuilder<DbMetric> builder)
        {
            builder.ToTable("Metrics");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.DisableComment).HasMaxLength(1000);
            builder.Property(t => t.ConditionAlarm).HasMaxLength(500);
            builder.Property(t => t.ConditionWarning).HasMaxLength(500);
            builder.Property(t => t.ConditionSuccess).HasMaxLength(500);
            builder.HasOne(t => t.Bulb).WithMany().HasForeignKey(t => t.StatusDataId).IsRequired();
            builder.HasOne(t => t.Component).WithMany(t => t.Metrics).HasForeignKey(t => t.ComponentId).IsRequired();
            builder.HasOne(t => t.MetricType).WithMany().HasForeignKey(t => t.MetricTypeId).IsRequired();
        }
    }
}
