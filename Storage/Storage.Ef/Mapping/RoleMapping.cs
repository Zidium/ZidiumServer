using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class RoleMapping : EntityTypeConfiguration<DbRole>
    {
        public RoleMapping()
        {
            HasKey(t => t.Id);
            ToTable("Roles");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SystemName).HasColumnName("SystemName").IsRequired().HasMaxLength(50);
            Property(t => t.DisplayName).HasColumnName("DisplayName").IsFixedLength().HasMaxLength(50);
        }
    }
}
