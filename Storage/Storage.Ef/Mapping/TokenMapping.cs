using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class TokenMapping : IEntityTypeConfiguration<DbToken>
    {
        public void Configure(EntityTypeBuilder<DbToken> builder)
        {
            builder.ToTable("Tokens");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.SecurityStamp).HasMaxLength(50);
            builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).IsRequired();
        }
    }
}
