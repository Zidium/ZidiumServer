using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    // Репозиторий для работы с тарифами
    public interface ITariffLimitRepository
    {
        TariffLimit Add(TariffLimit entity);

        TariffLimit GetById(Guid id);

        TariffLimit GetByIdOrNull(Guid id);

        IQueryable<TariffLimit> QueryAll();

        TariffLimit Update(TariffLimit entity);

        void Remove(TariffLimit entity);

        void Remove(Guid id);
    }
}
