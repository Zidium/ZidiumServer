using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class AccountSettingMapping : EntityTypeConfiguration<AccountSetting>
    {
        public AccountSettingMapping()
        {
            ToTable("AccountSettings");
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name").HasMaxLength(255).IsRequired();
            Property(t => t.Value).HasColumnName("Value").IsMaxLength();
        }
    }
}
