using System;

namespace Zidium.Storage
{
    public interface ITariffLimitRepository
    {
        void Add(TariffLimitForAdd entity);

        void Update(TariffLimitForUpdate entity);

        TariffLimitForRead GetOneById(Guid id);

        TariffLimitForRead GetOneOrNullById(Guid id);

        // TODO Use GetBaseTariffLimit()
        TariffLimitForRead GetHardTariffLimit();

        // TODO Use GetBaseTariffLimit()
        TariffLimitForRead GetSoftTariffLimit();

        TariffLimitForRead GetBaseTariffLimit(TariffLimitType type);

        TariffLimitForRead Find(TariffLimitType type, TariffLimitSource source);

    }
}
