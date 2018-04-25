using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb
{
    internal class AccountTariffMapping : EntityTypeConfiguration<AccountTariff>
    {
        public AccountTariffMapping()
        {
            ToTable("AccountTariffs");
            HasKey(t => t.Id);

            HasRequired(t => t.TariffLimit).WithMany().HasForeignKey(t => t.TariffLimitId);
        }
    }
}
