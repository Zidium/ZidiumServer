using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UserMapping : IEntityTypeConfiguration<DbUser>
    {
        public void Configure(EntityTypeBuilder<DbUser> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Login).IsRequired().HasMaxLength(255);
            builder.Property(t => t.EMail).HasMaxLength(255);
            builder.Property(t => t.PasswordHash).HasMaxLength(255);
            builder.Property(t => t.DisplayName).HasMaxLength(255);
            builder.Property(t => t.SecurityStamp).HasMaxLength(50);

            builder.HasIndex(t => t.Login);
        }
    }
}
