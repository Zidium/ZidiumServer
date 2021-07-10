using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class DefectMapping : IEntityTypeConfiguration<DbDefect>
    {
        public void Configure(EntityTypeBuilder<DbDefect> builder)
        {
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.ToTable("Defects");
            builder.Property(t => t.Notes).HasMaxLength(1000);
            builder.Property(t => t.Title).HasMaxLength(500);
            builder.HasOne(t => t.EventType).WithMany().HasForeignKey(t => t.EventTypeId);
            builder.HasOne(t => t.ResponsibleUser).WithMany().HasForeignKey(t => t.ResponsibleUserId);
            builder.HasOne(t => t.LastChange).WithMany().HasForeignKey(t => t.LastChangeId);
        }
    }
}
