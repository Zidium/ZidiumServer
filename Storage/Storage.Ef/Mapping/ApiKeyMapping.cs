using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class ApiKeyMapping : IEntityTypeConfiguration<DbApiKey>
    {
        public void Configure(EntityTypeBuilder<DbApiKey> builder)
        {
            builder.ToTable("ApiKeys");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Name).HasMaxLength(255).IsRequired();
            builder.Property(t => t.Value).HasMaxLength(255).IsRequired();
            builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
        }
    }
}
