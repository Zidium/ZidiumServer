using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class LogMapping : EntityTypeConfiguration<Log>
    {
        public LogMapping()
        {
            ToTable("Logs");
            HasKey(t => t.Id);
            Property(t => t.Message).HasMaxLength(8000);
            Property(t => t.Context).HasMaxLength(255);

            HasRequired(t => t.Component).WithMany().HasForeignKey(t => t.ComponentId);
            
            Property(t => t.ComponentId).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ComponentBased", 1)));
            Property(t => t.Date).HasColumnAnnotation("Index", new IndexAnnotation(new []
            {
                new IndexAttribute("IX_ComponentBased", 2),
                new IndexAttribute("IX_Date", 1)
            }));
            Property(t => t.Order).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ComponentBased", 3)));
            Property(t => t.Level).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ComponentBased", 4)));
            Property(t => t.Context).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ComponentBased", 5)));
        }
    }
}
