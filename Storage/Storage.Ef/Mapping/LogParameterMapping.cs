using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class LogParameterMapping : EntityTypeConfiguration<DbLogProperty>
    {
        public LogParameterMapping()
        {
            ToTable("LogParameters");
            HasKey(t => t.Id);
            Property(t => t.Name).HasMaxLength(100).IsRequired();
            Property(t => t.Value).IsMaxLength();
            HasRequired(t => t.Log).WithMany(t => t.Parameters).HasForeignKey(t => t.LogId);
        }
    }
}
