using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class ComponentMapping : IEntityTypeConfiguration<DbComponent>
    {
        public void Configure(EntityTypeBuilder<DbComponent> builder)
        {
            builder.ToTable("Components");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.DisplayName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Version).HasMaxLength(255);
            builder.Property(t => t.DisableComment).HasMaxLength(1000);

            builder.HasOne(t => t.Parent).WithMany(t => t.Childs).HasForeignKey(t => t.ParentId);
            builder.HasOne(t => t.ComponentType).WithMany(t => t.Components).HasForeignKey(t => t.ComponentTypeId).IsRequired();

            builder.HasMany(t => t.Metrics).WithOne(t => t.Component).HasForeignKey(t => t.ComponentId).IsRequired();
            builder.HasOne(t => t.InternalStatus).WithMany().HasForeignKey(d => d.InternalStatusId).IsRequired();
            builder.HasOne(t => t.ExternalStatus).WithMany().HasForeignKey(d => d.ExternalStatusId).IsRequired();
            builder.HasOne(t => t.UnitTestsStatus).WithMany().HasForeignKey(d => d.UnitTestsStatusId).IsRequired();
            builder.HasOne(t => t.EventsStatus).WithMany().HasForeignKey(d => d.EventsStatusId).IsRequired();
            builder.HasOne(t => t.MetricsStatus).WithMany().HasForeignKey(d => d.MetricsStatusId).IsRequired();
            builder.HasOne(t => t.ChildComponentsStatus).WithMany().HasForeignKey(d => d.ChildComponentsStatusId).IsRequired();

            builder.HasIndex(t => t.SystemName);
        }
    }
}

