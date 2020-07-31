using System;

namespace Zidium.Storage
{
    // TODO Deprecated
    /// <summary>
    /// Элемент тарифа для аккаунта
    /// </summary>
    public class AccountTariffForRead
    {
        public AccountTariffForRead(Guid id, Guid tariffLimitId)
        {
            Id = id;
            TariffLimitId = tariffLimitId;
        }

        public Guid Id { get; }

        public Guid TariffLimitId { get; }

    }
}
