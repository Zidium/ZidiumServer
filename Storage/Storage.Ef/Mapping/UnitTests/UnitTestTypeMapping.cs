using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestTypeMapping : IEntityTypeConfiguration<DbUnitTestType>
    {
        public void Configure(EntityTypeBuilder<DbUnitTestType> builder)
        {
            builder.ToTable("UnitTestTypes");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.DisplayName).IsRequired().HasMaxLength(255);

            builder.HasIndex(t => t.SystemName);
        }
    }
}
