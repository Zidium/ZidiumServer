using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class MetricHistoryMapping : IEntityTypeConfiguration<DbMetricHistory>
    {
        public void Configure(EntityTypeBuilder<DbMetricHistory> builder)
        {
            builder.ToTable("MetricHistory");
            builder.HasKey(t => t.Id).IsClustered(false);

            builder.HasOne(t => t.Component).WithMany().HasForeignKey(t => t.ComponentId).IsRequired();
            builder.HasOne(t => t.MetricType).WithMany().HasForeignKey(t => t.MetricTypeId).IsRequired();
            builder.HasOne(t => t.StatusEvent).WithMany().HasForeignKey(t => t.StatusEventId);

            builder.HasIndex(t => new { t.ComponentId, t.MetricTypeId, t.BeginDate }, "IX_ForHistory");
            builder.HasIndex(t => t.BeginDate);
            builder.HasIndex(t => t.StatusEventId);
        }
    }
}
