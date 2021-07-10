using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class MetricTypeRepository : IMetricTypeRepository
    {
        public MetricTypeRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(MetricTypeForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.MetricTypes.Add(new DbMetricType()
                {
                    Id = entity.Id,
                    DisplayName = entity.DisplayName,
                    SystemName = entity.SystemName,
                    CreateDate = entity.CreateDate,
                    IsDeleted = entity.IsDeleted,
                    ConditionAlarm = entity.ConditionAlarm,
                    ActualTimeSecs = entity.ActualTimeSecs,
                    ConditionSuccess = entity.ConditionSuccess,
                    NoSignalColor = entity.NoSignalColor,
                    ConditionWarning = entity.ConditionWarning,
                    ConditionElseColor = entity.ConditionElseColor
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(MetricTypeForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var metricType = DbGetOneById(entity.Id);

                if (entity.SystemName.Changed())
                    metricType.SystemName = entity.SystemName.Get();

                if (entity.DisplayName.Changed())
                    metricType.DisplayName = entity.DisplayName.Get();

                if (entity.IsDeleted.Changed())
                    metricType.IsDeleted = entity.IsDeleted.Get();

                if (entity.ConditionAlarm.Changed())
                    metricType.ConditionAlarm = entity.ConditionAlarm.Get();

                if (entity.ConditionWarning.Changed())
                    metricType.ConditionWarning = entity.ConditionWarning.Get();

                if (entity.ConditionSuccess.Changed())
                    metricType.ConditionSuccess = entity.ConditionSuccess.Get();

                if (entity.ConditionElseColor.Changed())
                    metricType.ConditionElseColor = entity.ConditionElseColor.Get();

                if (entity.NoSignalColor.Changed())
                    metricType.NoSignalColor = entity.NoSignalColor.Get();

                if (entity.ActualTimeSecs.Changed())
                    metricType.ActualTimeSecs = entity.ActualTimeSecs.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(MetricTypeForUpdate[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    var metricType = DbGetOneById(entity.Id);

                    if (entity.SystemName.Changed())
                        metricType.SystemName = entity.SystemName.Get();

                    if (entity.DisplayName.Changed())
                        metricType.DisplayName = entity.DisplayName.Get();

                    if (entity.IsDeleted.Changed())
                        metricType.IsDeleted = entity.IsDeleted.Get();

                    if (entity.ConditionAlarm.Changed())
                        metricType.ConditionAlarm = entity.ConditionAlarm.Get();

                    if (entity.ConditionWarning.Changed())
                        metricType.ConditionWarning = entity.ConditionWarning.Get();

                    if (entity.ConditionSuccess.Changed())
                        metricType.ConditionSuccess = entity.ConditionSuccess.Get();

                    if (entity.ConditionElseColor.Changed())
                        metricType.ConditionElseColor = entity.ConditionElseColor.Get();

                    if (entity.NoSignalColor.Changed())
                        metricType.NoSignalColor = entity.NoSignalColor.Get();

                    if (entity.ActualTimeSecs.Changed())
                        metricType.ActualTimeSecs = entity.ActualTimeSecs.Get();
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public MetricTypeForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public MetricTypeForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public MetricTypeForRead[] GetMany(Guid[] ids)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.MetricTypes.AsNoTracking()
                    .Where(t => ids.Contains(t.Id))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public MetricTypeForRead GetOneOrNullBySystemName(string systemName)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.MetricTypes.AsNoTracking()
                    .FirstOrDefault(t => !t.IsDeleted && t.SystemName.ToLower() == systemName.ToLower()));
            }
        }

        public MetricTypeForRead[] Filter(string search, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.MetricTypes.AsNoTracking()
                    .Where(t => !t.IsDeleted);

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(t => t.Id.ToString().ToLower() == search || t.SystemName.ToLower().Contains(search));
                }

                query = query.OrderBy(t => t.DisplayName).Take(maxCount);

                return query
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public int GetCount()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.MetricTypes.Count(t => !t.IsDeleted);
            }
        }

        private DbMetricType DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.MetricTypes.Find(id);
            }
        }

        private DbMetricType DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Тип метрики {id} не найден");

            return result;
        }

        private MetricTypeForRead DbToEntity(DbMetricType entity)
        {
            if (entity == null)
                return null;

            return new MetricTypeForRead(entity.Id, entity.SystemName, entity.DisplayName, entity.IsDeleted, entity.CreateDate,
                entity.ConditionAlarm, entity.ConditionWarning, entity.ConditionSuccess, entity.ConditionElseColor,
                entity.NoSignalColor, entity.ActualTimeSecs);
        }
    }
}
