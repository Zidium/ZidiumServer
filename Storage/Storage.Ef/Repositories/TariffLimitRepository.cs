using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class TariffLimitRepository : ITariffLimitRepository
    {
        public TariffLimitRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(TariffLimitForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.TariffLimits.Add(new DbTariffLimit()
                {
                    Id = entity.Id,
                    Type = entity.Type,
                    Name = entity.Name,
                    ComponentTypesMax = entity.ComponentTypesMax,
                    ComponentsMax = entity.ComponentsMax,
                    Description = entity.Description,
                    EventsMaxDays = entity.EventsMaxDays,
                    EventsRequestsPerDay = entity.EventsRequestsPerDay,
                    HttpUnitTestsMaxNoBanner = entity.HttpUnitTestsMaxNoBanner,
                    LogMaxDays = entity.LogMaxDays,
                    LogSizePerDay = entity.LogSizePerDay,
                    MetricsMax = entity.MetricsMax,
                    MetricsMaxDays = entity.MetricsMaxDays,
                    MetricsRequestsPerDay = entity.MetricsRequestsPerDay,
                    SmsPerDay = entity.SmsPerDay,
                    Source = entity.Source,
                    StorageSizeMax = entity.StorageSizeMax,
                    UnitTestTypesMax = entity.UnitTestTypesMax,
                    UnitTestsMax = entity.UnitTestsMax,
                    UnitTestsMaxDays = entity.UnitTestsMaxDays,
                    UnitTestsRequestsPerDay = entity.UnitTestsRequestsPerDay
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(TariffLimitForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var tariffLimit = DbGetOneById(entity.Id);

                if (entity.EventsRequestsPerDay.Changed())
                    tariffLimit.EventsRequestsPerDay = entity.EventsRequestsPerDay.Get();

                if (entity.EventsMaxDays.Changed())
                    tariffLimit.EventsMaxDays = entity.EventsMaxDays.Get();

                if (entity.LogSizePerDay.Changed())
                    tariffLimit.LogSizePerDay = entity.LogSizePerDay.Get();

                if (entity.LogMaxDays.Changed())
                    tariffLimit.LogMaxDays = entity.LogMaxDays.Get();

                if (entity.UnitTestsRequestsPerDay.Changed())
                    tariffLimit.UnitTestsRequestsPerDay = entity.UnitTestsRequestsPerDay.Get();

                if (entity.UnitTestsMaxDays.Changed())
                    tariffLimit.UnitTestsMaxDays = entity.UnitTestsMaxDays.Get();

                if (entity.MetricsRequestsPerDay.Changed())
                    tariffLimit.MetricsRequestsPerDay = entity.MetricsRequestsPerDay.Get();

                if (entity.MetricsMaxDays.Changed())
                    tariffLimit.MetricsMaxDays = entity.MetricsMaxDays.Get();

                if (entity.ComponentsMax.Changed())
                    tariffLimit.ComponentsMax = entity.ComponentsMax.Get();

                if (entity.ComponentTypesMax.Changed())
                    tariffLimit.ComponentTypesMax = entity.ComponentTypesMax.Get();

                if (entity.UnitTestTypesMax.Changed())
                    tariffLimit.UnitTestTypesMax = entity.UnitTestTypesMax.Get();

                if (entity.HttpUnitTestsMaxNoBanner.Changed())
                    tariffLimit.HttpUnitTestsMaxNoBanner = entity.HttpUnitTestsMaxNoBanner.Get();

                if (entity.UnitTestsMax.Changed())
                    tariffLimit.UnitTestsMax = entity.UnitTestsMax.Get();

                if (entity.MetricsMax.Changed())
                    tariffLimit.MetricsMax = entity.MetricsMax.Get();

                if (entity.StorageSizeMax.Changed())
                    tariffLimit.StorageSizeMax = entity.StorageSizeMax.Get();

                if (entity.SmsPerDay.Changed())
                    tariffLimit.SmsPerDay = entity.SmsPerDay.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public TariffLimitForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public TariffLimitForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public TariffLimitForRead GetHardTariffLimit()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.TariffLimits.AsNoTracking()
                    .FirstOrDefault(t => t.Type == TariffLimitType.Hard && t.Source == TariffLimitSource.Base));
            }
        }

        public TariffLimitForRead GetSoftTariffLimit()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.TariffLimits.AsNoTracking()
                    .FirstOrDefault(t => t.Type == TariffLimitType.Soft && t.Source == TariffLimitSource.Base));
            }
        }

        public TariffLimitForRead GetBaseTariffLimit(TariffLimitType type)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.TariffLimits.AsNoTracking()
                    .FirstOrDefault(t => t.Type == type && t.Source == TariffLimitSource.Base));
            }
        }

        public TariffLimitForRead Find(TariffLimitType type, TariffLimitSource source)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.TariffLimits.AsNoTracking()
                    .FirstOrDefault(t => t.Type == type && t.Source == source));
            }
        }

        private DbTariffLimit DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.TariffLimits.Find(id);
            }
        }

        private DbTariffLimit DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Тариф {id} не найден");

            return result;
        }

        private TariffLimitForRead DbToEntity(DbTariffLimit entity)
        {
            if (entity == null)
                return null;

            return new TariffLimitForRead(entity.Id, entity.Name, entity.Description, entity.Type, entity.Source,
                entity.EventsRequestsPerDay, entity.EventsMaxDays, entity.LogSizePerDay, entity.LogMaxDays,
                entity.UnitTestsRequestsPerDay, entity.UnitTestsMaxDays, entity.MetricsRequestsPerDay,
                entity.MetricsMaxDays, entity.ComponentsMax, entity.ComponentTypesMax, entity.UnitTestTypesMax,
                entity.HttpUnitTestsMaxNoBanner, entity.UnitTestsMax, entity.MetricsMax, entity.StorageSizeMax,
                entity.SmsPerDay);
        }
    }
}
