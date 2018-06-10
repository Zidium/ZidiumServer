using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class MetricHistoryMapping : EntityTypeConfiguration<MetricHistory>
    {
        public MetricHistoryMapping()
        {
            ToTable("MetricHistory");
            HasKey(t => t.Id);
            Property(t => t.HasSignal).HasColumnName("HasSignal");

            HasRequired(t => t.Component).WithMany().HasForeignKey(t => t.ComponentId);
            HasRequired(t => t.MetricType).WithMany().HasForeignKey(t => t.MetricTypeId);
            HasOptional(t => t.StatusEvent).WithMany().HasForeignKey(t => t.StatusEventId);

            Property(t => t.ComponentId).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ForHistory", 1)));
            Property(t => t.MetricTypeId).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ForHistory", 2)));
            Property(t => t.BeginDate).HasColumnAnnotation("Index", new IndexAnnotation(new[] {
                new IndexAttribute("IX_ForHistory", 3),
                new IndexAttribute("IX_BeginDate", 1)
            }));
            Property(t => t.StatusEventId).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_StatusEventId", 1)));
        }
    }
}
