using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UserContactMapping : IEntityTypeConfiguration<DbUserContact>
    {
        public void Configure(EntityTypeBuilder<DbUserContact> builder)
        {
            builder.ToTable("UserContacts");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Value).IsRequired().HasMaxLength(255);
            builder.HasOne(t => t.User).WithMany(t => t.UserContacts).HasForeignKey(d => d.UserId).IsRequired();
        }
    }
}
