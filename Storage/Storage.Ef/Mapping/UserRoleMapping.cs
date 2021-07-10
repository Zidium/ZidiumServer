using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UserRoleMapping : IEntityTypeConfiguration<DbUserRole>
    {
        public void Configure(EntityTypeBuilder<DbUserRole> builder)
        {
            builder.ToTable("UserRoles");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.HasOne(t => t.User).WithMany(t => t.Roles).HasForeignKey(t => t.UserId).IsRequired();
            builder.HasOne(t => t.Role).WithMany().HasForeignKey(t => t.RoleId).IsRequired();
        }
    }
}
