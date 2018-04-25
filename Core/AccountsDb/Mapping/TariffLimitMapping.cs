using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    
    internal class TariffLimitMapping : EntityTypeConfiguration<TariffLimit>
    {
        public TariffLimitMapping()
        {
            HasKey(t => t.Id);
            ToTable("TariffLimits");
            Property(t => t.Name).IsRequired().HasMaxLength(255);
            Property(t => t.Description).HasMaxLength(4000);
        }
    }
}
