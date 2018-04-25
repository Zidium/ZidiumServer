using System;
using System.Linq;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    // Репозиторий для работы с тарифами
    public class TariffLimitRepository : ITariffLimitRepository
    {
        protected AccountDbContext Context;

        // Создание нового репозитория для указанного контекста
        public TariffLimitRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public TariffLimit Add(TariffLimit entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            Context.TariffLimits.Add(entity);
            return entity;
        }

        public TariffLimit GetById(Guid id)
        {
            var entity = GetByIdOrNull(id);

            if (entity == null)
                throw new ObjectNotFoundException(id, Naming.TariffLimit);

            return entity;
        }

        public TariffLimit GetByIdOrNull(Guid id)
        {
            return Context.TariffLimits.FirstOrDefault(t => t.Id == id);
        }

        public IQueryable<TariffLimit> QueryAll()
        {
            return Context.TariffLimits;
        }

        public TariffLimit Update(TariffLimit entity)
        {
            Context.SaveChanges();
            return entity;
        }

        public void Remove(TariffLimit entity)
        {
            Context.TariffLimits.Remove(entity);
            Context.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            Remove(entity);
        }
    }
}
