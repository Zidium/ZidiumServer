using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class RoleMapping : IEntityTypeConfiguration<DbRole>
    {
        public void Configure(EntityTypeBuilder<DbRole> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.SystemName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.DisplayName).IsFixedLength().HasMaxLength(50);
        }
    }
}
