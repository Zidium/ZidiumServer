using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class ComponentPropertyMapping : IEntityTypeConfiguration<DbComponentProperty>
    {
        public void Configure(EntityTypeBuilder<DbComponentProperty> builder)
        {
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.ToTable("ComponentProperties");
            builder.Property(t => t.Name).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Value).HasMaxLength(8000);
            builder.HasOne(x => x.Component).WithMany(x => x.Properties).HasForeignKey(x => x.ComponentId).IsRequired();
        }
    }
}
