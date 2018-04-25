using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UserSettingMapping : EntityTypeConfiguration<UserSetting>
    {
        public UserSettingMapping()
        {
            ToTable("UserSettings");
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.Name).HasColumnName("Name").HasMaxLength(255).IsRequired();
            Property(t => t.Value).HasColumnName("Value").HasMaxLength(255);
            HasRequired(t => t.User).WithMany(t => t.Settings).HasForeignKey(t => t.UserId);
        }
    }
}
