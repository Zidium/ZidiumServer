using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class ComponentTypeMapping : EntityTypeConfiguration<ComponentType>
    {
        public ComponentTypeMapping()
        {
            HasKey(t => t.Id);
            ToTable("ComponentTypes");
            Property(t => t.DisplayName).IsRequired().HasMaxLength(255);
            Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            Property(t => t.IsDeleted).IsRequired();
            Property(t => t.SystemName).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
        }
    }
}
