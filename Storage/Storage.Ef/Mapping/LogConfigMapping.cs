using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class LogConfigMapping : IEntityTypeConfiguration<DbLogConfig>
    {
        public void Configure(EntityTypeBuilder<DbLogConfig> builder)
        {
            builder.ToTable("LogConfigs");
            builder.HasKey(t => t.ComponentId).IsClustered(false);
            builder.HasOne(t => t.Component).WithOne(t => t.LogConfig).IsRequired();
        }
    }
}
