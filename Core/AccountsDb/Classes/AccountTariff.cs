using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Элемент тарифа для аккаунта
    /// </summary>
    public class AccountTariff
    {
        public Guid Id { get; set; }

        public Guid TariffLimitId { get; set; }

        public virtual TariffLimit TariffLimit { get; set; }
    }
}
