using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestPropertyMapping : IEntityTypeConfiguration<DbUnitTestProperty>
    {
        public void Configure(EntityTypeBuilder<DbUnitTestProperty> builder)
        {
            builder.ToTable("UnitTestProperties");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Value).HasMaxLength(8000);
            builder.HasOne(x => x.UnitTest).WithMany(x => x.Properties).HasForeignKey(x => x.UnitTestId).IsRequired();
        }
    }
}
