using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    
    internal class UserContactMapping : EntityTypeConfiguration<UserContact>
    {
        public UserContactMapping()
        {
            HasKey(t => t.Id);
            ToTable("UserContacts");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.Type).HasColumnName("Type");
            Property(t => t.Value).HasColumnName("Value").IsRequired().HasMaxLength(255);
            HasRequired(t => t.User).WithMany(t => t.UserContacts).HasForeignKey(d => d.UserId);
         }
    }
}

