using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class EventTypeMapping : EntityTypeConfiguration<DbEventType>
    {
        public EventTypeMapping()
        {
            HasKey(t => t.Id);
            ToTable("EventTypes");
            Property(t => t.DisplayName).IsRequired().HasMaxLength(255);
            Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            Property(t => t.Code).HasMaxLength(20);
            Property(t => t.OldVersion).HasMaxLength(255);
            Property(t => t.SystemName).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
        }
    }
}
