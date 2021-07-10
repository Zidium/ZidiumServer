using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class LogParameterMapping : IEntityTypeConfiguration<DbLogProperty>
    {
        public void Configure(EntityTypeBuilder<DbLogProperty> builder)
        {
            builder.ToTable("LogParameters");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
            builder.HasOne(t => t.Log).WithMany(t => t.Parameters).HasForeignKey(t => t.LogId).IsRequired();
        }
    }
}
