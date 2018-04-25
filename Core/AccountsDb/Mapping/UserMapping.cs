using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UserMapping : EntityTypeConfiguration<User>
    {
        public UserMapping()
        {
            ToTable("Users");
            HasKey(t => t.Id);
            Property(t => t.Login).IsRequired().HasMaxLength(255);
            Property(t => t.PasswordHash).HasMaxLength(255);
            Property(t => t.FirstName).HasMaxLength(100);
            Property(t => t.LastName).HasMaxLength(100);
            Property(t => t.MiddleName).HasMaxLength(100);
            Property(t => t.DisplayName).HasMaxLength(100);
            Property(t => t.Post).HasMaxLength(100);
            Property(t => t.SecurityStamp).HasMaxLength(50);

            Property(t => t.Login).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
        }
    }
}
