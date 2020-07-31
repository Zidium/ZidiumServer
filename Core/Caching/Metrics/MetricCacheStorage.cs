using System;
using System.Linq;
using Zidium.Api.Others;
using Zidium.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class MetricCacheStorage : AccountDbCacheStorageBase<MetricCacheResponse, IMetricCacheReadObject, MetricCacheWriteObject>
    {
        protected override Exception CreateNotFoundException(AccountCacheRequest request)
        {
            return new UserFriendlyException("Не удалось найти метрику с ID " + request.ObjectId);
        }

        protected override void ValidateChanges(MetricCacheWriteObject oldObj, MetricCacheWriteObject newObj)
        {
            if (newObj.AccountId == Guid.Empty)
            {
                throw new Exception("newObj.AccountId == Guid.Empty");
            }
            if (newObj.ComponentId == Guid.Empty)
            {
                throw new Exception("newObj.ComponentId == Guid.Empty");
            }
            if (newObj.CreateDate == DateTime.MinValue)
            {
                throw new Exception("newObj.CreateDate == DateTime.MinValue");
            }
            if (newObj.StatusDataId == Guid.Empty)
            {
                throw new Exception("newObj.StatusDataId == Guid.Empty");
            }

            if (oldObj.AccountId != newObj.AccountId)
            {
                throw new Exception("oldObj.AccountId != newObj.AccountId");
            }
            if (StringHelper.GetLength(newObj.DisableComment) > 1000)
            {
                throw new UserFriendlyException("Длина DisableComment макимум 1000 символов");
            }

            var newComponentRequest = new AccountCacheRequest()
            {
                AccountId = newObj.AccountId,
                ObjectId = newObj.ComponentId
            };

            // если удалена
            if (newObj.IsDeleted)
            {
                // получим нового родителя
                using (var newComponent = AllCaches.Components.Write(newComponentRequest))
                {
                    newComponent.WriteMetrics.Delete(newObj.Id);
                    newComponent.BeginSave();
                }
            }
            // если изменился родитель
            else if (oldObj.ComponentId != newObj.ComponentId)
            {
                // получим нового родителя
                using (var newComponent = AllCaches.Components.Write(newComponentRequest))
                {
                    // проверим, что у нового родителя нет ребенка с таким же именем
                    var child = newComponent.Metrics.FindByMetricTypeId(newObj.MetricTypeId);
                    if (child != null)
                    {
                        throw new Exception("У нового компонента уже есть метрика с таким именем");
                    }

                    // получим старого родителя
                    var oldComponentRequest = new AccountCacheRequest()
                    {
                        AccountId = newObj.AccountId,
                        ObjectId = oldObj.ComponentId
                    };
                    using (var oldComponent = AllCaches.Components.Write(oldComponentRequest))
                    {
                        var reference = new CacheObjectReference(newObj.Id, newObj.MetricTypeId.ToString());
                        newComponent.WriteMetrics.Add(reference);
                        oldComponent.WriteMetrics.Delete(newObj.Id);
                        oldComponent.BeginSave();
                    }
                    newComponent.BeginSave();
                }
            }
            // если изменилось только системное имя
            else if (oldObj.MetricTypeId != newObj.MetricTypeId)
            {
                // проверим что у компонента нет проверки с таким MetricTypeId
                using (var component = AllCaches.Components.Write(newComponentRequest))
                {
                    var child = component.Metrics.FindByMetricTypeId(newObj.MetricTypeId);
                    if (child != null)
                    {
                        throw new Exception("У компонента уже есть метрика с таким именем");
                    }
                    component.WriteMetrics.Rename(oldObj.MetricTypeId.ToString(), newObj.MetricTypeId.ToString());
                    component.BeginSave();
                }
            }
        }

        protected override bool HasChanges(MetricCacheWriteObject oldObj, MetricCacheWriteObject newObj)
        {
            return ObjectChangesHelper.HasChanges(oldObj, newObj);
        }

        protected override void AddBatchObjects(IStorage storage, MetricCacheWriteObject[] writeObjects)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObjects(IStorage storage, MetricCacheWriteObject[] metrics, bool useCheck)
        {
            var entities = metrics.Select(metric =>
            {
                var lastData = metric.Response.LastSavedData;
                if (lastData == null)
                {
                    throw new Exception("oldMetric == null");
                }

                var entity = lastData.CreateEf();

                if (lastData.ActualDate != metric.ActualDate)
                    entity.ActualDate.Set(metric.ActualDate);

                if (lastData.BeginDate != metric.BeginDate)
                    entity.BeginDate.Set(metric.BeginDate);

                if (lastData.Enable != metric.Enable)
                    entity.Enable.Set(metric.Enable);

                if (lastData.DisableComment != metric.DisableComment)
                    entity.DisableComment.Set(metric.DisableComment);

                if (lastData.DisableToDate != metric.DisableToDate)
                    entity.DisableToDate.Set(metric.DisableToDate);

                if (lastData.IsDeleted != metric.IsDeleted)
                    entity.IsDeleted.Set(metric.IsDeleted);

                if (lastData.ParentEnable != metric.ParentEnable)
                    entity.ParentEnable.Set(metric.ParentEnable);

                if (lastData.StatusDataId != metric.StatusDataId)
                    entity.StatusDataId.Set(metric.StatusDataId);

                if (lastData.Value != metric.Value)
                    entity.Value.Set(metric.Value);

                if (lastData.ConditionRed != metric.ConditionRed)
                    entity.ConditionAlarm.Set(metric.ConditionRed);

                if (lastData.ConditionYellow != metric.ConditionYellow)
                    entity.ConditionWarning.Set(metric.ConditionYellow);

                if (lastData.ConditionGreen != metric.ConditionGreen)
                    entity.ConditionSuccess.Set(metric.ConditionGreen);

                if (lastData.ActualTime != metric.ActualTime)
                    entity.ActualTimeSecs.Set(TimeSpanHelper.GetSeconds(metric.ActualTime));

                if (lastData.NoSignalColor != metric.NoSignalColor)
                    entity.NoSignalColor.Set(metric.NoSignalColor);

                if (lastData.ElseColor != metric.ElseColor)
                    entity.ConditionElseColor.Set(metric.ElseColor);

                return entity;
            }).ToArray();

            storage.Metrics.Update(entities);
        }

        public override int BatchCount
        {
            get { return 100; }
        }

        protected override MetricCacheWriteObject LoadObject(AccountCacheRequest request, IStorage storage)
        {
            var metric = storage.Metrics.GetOneOrNullById(request.ObjectId);
            return MetricCacheWriteObject.Create(metric, request.AccountId);
        }
    }
}
