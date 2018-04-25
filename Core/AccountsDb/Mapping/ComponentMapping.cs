using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class ComponentMapping : EntityTypeConfiguration<Component>
    {
        public ComponentMapping()
        {
            HasKey(t => t.Id);
            ToTable("Components");
            Property(t => t.DisplayName).IsRequired().HasMaxLength(255);
            Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            Property(t => t.Version).HasMaxLength(255);
            Property(t => t.DisableComment).HasMaxLength(1000);

            HasOptional(t => t.Parent).WithMany(t => t.Childs).HasForeignKey(t => t.ParentId);
            HasRequired(t => t.ComponentType).WithMany(t => t.Components).HasForeignKey(t => t.ComponentTypeId);

            Property(t => t.SystemName).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
            HasMany(t => t.Metrics).WithRequired(t => t.Component).HasForeignKey(t => t.ComponentId);
            HasRequired(t => t.InternalStatus).WithMany().HasForeignKey(d => d.InternalStatusId);
            HasRequired(t => t.ExternalStatus).WithMany().HasForeignKey(d => d.ExternalStatusId);
            HasRequired(t => t.UnitTestsStatus).WithMany().HasForeignKey(d => d.UnitTestsStatusId);
            HasRequired(t => t.EventsStatus).WithMany().HasForeignKey(d => d.EventsStatusId);
            HasRequired(t => t.MetricsStatus).WithMany().HasForeignKey(d => d.MetricsStatusId);
            HasRequired(t => t.ChildComponentsStatus).WithMany().HasForeignKey(d => d.ChildComponentsStatusId);
        }
    }
}

