using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class EventParameterMapping: EntityTypeConfiguration<EventProperty>
    {
        public EventParameterMapping()
        {
            ToTable("EventParameters");
            HasKey(t => t.Id);
            Property(t => t.Name).IsRequired().HasMaxLength(100);
            
            HasRequired(t => t.Event).WithMany(t => t.Properties).HasForeignKey(d => d.EventId);
         }
    }
}
