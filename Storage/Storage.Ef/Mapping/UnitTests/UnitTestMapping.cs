using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestMapping : IEntityTypeConfiguration<DbUnitTest>
    {
        public void Configure(EntityTypeBuilder<DbUnitTest> builder)
        {
            builder.ToTable("UnitTests");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.DisplayName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.DisableComment).HasMaxLength(1000);
            builder.HasOne(x => x.Type).WithMany().HasForeignKey(x => x.TypeId).IsRequired();
            builder.HasOne(x => x.Component).WithMany(x => x.UnitTests).HasForeignKey(x => x.ComponentId).IsRequired();
            builder.HasOne(x => x.Bulb).WithMany().HasForeignKey(x => x.StatusDataId).IsRequired();
        }
    }
}
