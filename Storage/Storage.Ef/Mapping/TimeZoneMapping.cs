using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class TimeZoneMapping : EntityTypeConfiguration<DbTimeZone>
    {
        public TimeZoneMapping()
        {
            HasKey(t => t.OffsetMinutes);
            ToTable("TimeZones");
            Property(t => t.OffsetMinutes).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(t => t.Name).IsRequired().HasMaxLength(255);
        }
    }
}