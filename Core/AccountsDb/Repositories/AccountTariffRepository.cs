using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class AccountTariffRepository : IAccountTariffRepository
    {
        protected AccountDbContext Context;

        public AccountTariffRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public AccountTariff Add(AccountTariff entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            Context.AccountTariffs.Add(entity);
            return entity;
        }

        public AccountTariff GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public AccountTariff GetByIdOrNull(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<AccountTariff> QueryAll()
        {
            return Context.AccountTariffs;
        }

        public AccountTariff Update(AccountTariff entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(AccountTariff entity)
        {
            Context.AccountTariffs.Remove(entity);
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public TariffLimit GetHardTariffLimit()
        {
            var tariffLimits = QueryAll().Where(t => t.TariffLimit.Type == TariffLimitType.Hard).ToList();
            var result = TariffLimitFromList(tariffLimits);
            return result;
        }

        public TariffLimit GetSoftTariffLimit()
        {
            var tariffLimits = QueryAll().Where(t => t.TariffLimit.Type == TariffLimitType.Soft).ToList();
            var result = TariffLimitFromList(tariffLimits);
            return result;
        }

        public TariffLimit GetBaseTariffLimit(TariffLimitType type)
        {
            return QueryAll().Select(t => t.TariffLimit).First(t => t.Type == type);
        }

        protected TariffLimit TariffLimitFromList(List<AccountTariff> tariffLimits)
        {
            var result = new TariffLimit()
            {
                ComponentsMax = tariffLimits.Sum(t => t.TariffLimit.ComponentsMax),
                ComponentTypesMax = tariffLimits.Sum(t => t.TariffLimit.ComponentTypesMax),
                UnitTestTypesMax = tariffLimits.Sum(t => t.TariffLimit.UnitTestTypesMax),
                HttpUnitTestsMaxNoBanner = tariffLimits.Sum(t => t.TariffLimit.HttpUnitTestsMaxNoBanner),
                UnitTestsMax = tariffLimits.Sum(t => t.TariffLimit.UnitTestsMax),
                UnitTestsRequestsPerDay = tariffLimits.Sum(t => t.TariffLimit.UnitTestsRequestsPerDay),
                UnitTestsMaxDays = tariffLimits.Sum(t => t.TariffLimit.UnitTestsMaxDays),
                LogMaxDays = tariffLimits.Sum(t => t.TariffLimit.LogMaxDays),
                LogSizePerDay = tariffLimits.Sum(t => t.TariffLimit.LogSizePerDay),
                EventsRequestsPerDay = tariffLimits.Sum(t => t.TariffLimit.EventsRequestsPerDay),
                EventsMaxDays = tariffLimits.Sum(t => t.TariffLimit.EventsMaxDays),
                MetricsMax = tariffLimits.Sum(t => t.TariffLimit.MetricsMax),
                MetricsRequestsPerDay = tariffLimits.Sum(t => t.TariffLimit.MetricsRequestsPerDay),
                MetricsMaxDays = tariffLimits.Sum(t => t.TariffLimit.MetricsMaxDays),
                StorageSizeMax = tariffLimits.Sum(t => t.TariffLimit.StorageSizeMax),
                SmsPerDay = tariffLimits.Sum(t => t.TariffLimit.SmsPerDay)
            };
            return result;            
        }
    }
}
