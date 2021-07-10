using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class DefectChangeMapping : IEntityTypeConfiguration<DbDefectChange>
    {
        public void Configure(EntityTypeBuilder<DbDefectChange> builder)
        {
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.ToTable("DefectChanges");
            builder.Property(t => t.Comment).HasMaxLength(1000);
            builder.HasOne(t => t.Defect).WithMany(t => t.Changes).HasForeignKey(t => t.DefectId).IsRequired();
            builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).IsRequired();
        }
    }
}
