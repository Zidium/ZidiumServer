using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class ComponentPropertyMapping : EntityTypeConfiguration<ComponentProperty>
    {
        public ComponentPropertyMapping()
        {
            HasKey(t => t.Id);
            ToTable("ComponentProperties");
            Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(255);
            Property(t => t.Value).HasColumnName("Value").HasMaxLength(8000);
            Property(t => t.DataType);
            HasRequired(x => x.Component).WithMany(x => x.Properties).HasForeignKey(x => x.ComponentId).WillCascadeOnDelete(false);
        }
    }
}
