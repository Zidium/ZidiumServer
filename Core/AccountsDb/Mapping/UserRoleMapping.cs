using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UserRoleMapping : EntityTypeConfiguration<UserRole>
    {
        public UserRoleMapping()
        {
            ToTable("UserRoles");
            HasKey(t => t.Id);
            HasRequired(t => t.User).WithMany(t => t.Roles).HasForeignKey(t => t.UserId).WillCascadeOnDelete(false);
            HasRequired(t => t.Role).WithMany().HasForeignKey(t => t.RoleId).WillCascadeOnDelete(false);
        }
    }
}
