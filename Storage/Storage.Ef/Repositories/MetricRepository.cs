using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class MetricRepository : IMetricRepository
    {
        public MetricRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(MetricForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Metrics.Add(new DbMetric()
                {
                    Id = entity.Id,
                    Value = entity.Value,
                    MetricTypeId = entity.MetricTypeId,
                    CreateDate = entity.CreateDate,
                    ComponentId = entity.ComponentId,
                    StatusDataId = entity.StatusDataId,
                    IsDeleted = entity.IsDeleted,
                    ActualDate = entity.ActualDate,
                    ActualTimeSecs = entity.ActualTimeSecs,
                    BeginDate = entity.BeginDate,
                    ConditionAlarm = entity.ConditionAlarm,
                    ConditionElseColor = entity.ConditionElseColor,
                    ConditionSuccess = entity.ConditionSuccess,
                    ConditionWarning = entity.ConditionWarning,
                    DisableComment = entity.DisableComment,
                    DisableToDate = entity.DisableToDate,
                    Enable = entity.Enable,
                    NoSignalColor = entity.NoSignalColor,
                    ParentEnable = entity.ParentEnable
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(MetricForUpdate[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    var metric = DbGetOneById(entity.Id);

                    if (entity.IsDeleted.Changed())
                        metric.IsDeleted = entity.IsDeleted.Get();

                    if (entity.DisableToDate.Changed())
                        metric.DisableToDate = entity.DisableToDate.Get();

                    if (entity.DisableComment.Changed())
                        metric.DisableComment = entity.DisableComment.Get();

                    if (entity.Enable.Changed())
                        metric.Enable = entity.Enable.Get();

                    if (entity.ParentEnable.Changed())
                        metric.ParentEnable = entity.ParentEnable.Get();

                    if (entity.Value.Changed())
                        metric.Value = entity.Value.Get();

                    if (entity.BeginDate.Changed())
                        metric.BeginDate = entity.BeginDate.Get();

                    if (entity.ActualDate.Changed())
                        metric.ActualDate = entity.ActualDate.Get();

                    if (entity.ActualTimeSecs.Changed())
                        metric.ActualTimeSecs = entity.ActualTimeSecs.Get();

                    if (entity.NoSignalColor.Changed())
                        metric.NoSignalColor = entity.NoSignalColor.Get();

                    if (entity.ConditionAlarm.Changed())
                        metric.ConditionAlarm = entity.ConditionAlarm.Get();

                    if (entity.ConditionWarning.Changed())
                        metric.ConditionWarning = entity.ConditionWarning.Get();

                    if (entity.ConditionSuccess.Changed())
                        metric.ConditionSuccess = entity.ConditionSuccess.Get();

                    if (entity.ConditionElseColor.Changed())
                        metric.ConditionElseColor = entity.ConditionElseColor.Get();

                    if (entity.StatusDataId.Changed())
                        metric.StatusDataId = entity.StatusDataId.Get();
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public MetricForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public MetricForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        public Guid[] GetNotActualIds(int maxCount, DateTime now)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Metrics
                    .Where(x => !x.IsDeleted && x.ActualDate < now)
                    .OrderBy(x => x.ActualDate)
                    .Take(maxCount)
                    .Select(t => t.Id)
                    .ToArray();
            }
        }

        public Guid[] GetByMetricTypeId(Guid metricTypeId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Metrics
                    .Where(t => !t.IsDeleted && t.MetricTypeId == metricTypeId)
                    .Select(t => t.Id)
                    .ToArray();
            }
        }

        public MetricForRead[] GetByComponentId(Guid componentId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Metrics.AsNoTracking()
                    .Where(t => !t.IsDeleted && t.ComponentId == componentId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public MetricForRead[] Filter(Guid? metricTypeId, Guid? componentId, MonitoringStatus[] statuses, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Metrics.AsNoTracking().Where(t => !t.IsDeleted);

                if (metricTypeId.HasValue)
                    query = query.Where(t => t.MetricTypeId == metricTypeId.Value);

                if (componentId.HasValue)
                    query = query.Where(t => t.ComponentId == componentId.Value);

                if (statuses != null && statuses.Length > 0)
                    query = query.Where(t => statuses.Contains(t.Bulb.Status));

                return query
                    .OrderBy(t => t.MetricType.DisplayName)
                    .Take(maxCount)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbMetric DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Metrics.Find(id);
            }
        }

        private DbMetric DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Метрика {id} не найдена");

            return result;
        }

        private MetricForRead DbToEntity(DbMetric entity)
        {
            if (entity == null)
                return null;

            return new MetricForRead(entity.Id, entity.ComponentId, entity.MetricTypeId, entity.IsDeleted, entity.DisableToDate,
                entity.DisableComment, entity.Enable, entity.ParentEnable, entity.CreateDate, entity.Value, entity.BeginDate,
                entity.ActualDate, entity.ActualTimeSecs, entity.NoSignalColor, entity.ConditionAlarm, entity.ConditionWarning,
                entity.ConditionSuccess, entity.ConditionElseColor, entity.StatusDataId);
        }
    }
}
