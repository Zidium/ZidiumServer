using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class LogMapping : IEntityTypeConfiguration<DbLog>
    {
        public void Configure(EntityTypeBuilder<DbLog> builder)
        {
            builder.ToTable("Logs");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Message).HasMaxLength(8000);
            builder.Property(t => t.Context).HasMaxLength(255);

            builder.HasOne(t => t.Component).WithMany().HasForeignKey(t => t.ComponentId).IsRequired();

            builder.HasIndex(t => new { t.ComponentId, t.Date, t.Order, t.Level, t.Context }, "IX_ComponentBased");
            builder.HasIndex(t => t.Date, "IX_Date");
        }
    }
}
