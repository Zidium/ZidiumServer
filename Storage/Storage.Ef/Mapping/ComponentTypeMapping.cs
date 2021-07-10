using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class ComponentTypeMapping : IEntityTypeConfiguration<DbComponentType>
    {
        public void Configure(EntityTypeBuilder<DbComponentType> builder)
        {
            builder.ToTable("ComponentTypes");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.DisplayName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.IsDeleted).IsRequired();

            builder.HasIndex(t => t.SystemName);
        }
    }
}
