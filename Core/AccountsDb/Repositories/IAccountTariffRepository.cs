using System;

namespace Zidium.Core.AccountsDb
{
    public interface IAccountTariffRepository : IAccountBasedRepository<AccountTariff>
    {
        TariffLimit GetHardTariffLimit();

        TariffLimit GetSoftTariffLimit();

        TariffLimit GetBaseTariffLimit(TariffLimitType type);
    }
}
