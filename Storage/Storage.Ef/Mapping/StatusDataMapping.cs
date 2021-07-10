using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class StatusDataMapping : IEntityTypeConfiguration<DbBulb>
    {
        public void Configure(EntityTypeBuilder<DbBulb> builder)
        {
            builder.ToTable("Bulbs");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Message).HasMaxLength(8000);
            builder.HasOne(t => t.Component).WithMany().HasForeignKey(t => t.ComponentId);
            builder.HasOne(t => t.UnitTest).WithMany().HasForeignKey(t => t.UnitTestId);
            builder.HasOne(t => t.Metric).WithMany().HasForeignKey(t => t.MetricId);
            builder.Property(t => t.LastChildBulbId).HasColumnName("LastChildStatusDataId");
        }
    }
}

