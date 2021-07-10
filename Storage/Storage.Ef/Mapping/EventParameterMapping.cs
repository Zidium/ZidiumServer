using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class EventParameterMapping: IEntityTypeConfiguration<DbEventProperty>
    {
        public void Configure(EntityTypeBuilder<DbEventProperty> builder)
        {
            builder.ToTable("EventParameters");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);

            builder.HasOne(t => t.Event).WithMany(t => t.Properties).HasForeignKey(d => d.EventId).IsRequired();
        }
    }
}
