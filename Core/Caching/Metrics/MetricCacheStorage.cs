using System;
using Zidium.Api.Others;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

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

        protected override void AddBatchObject(AccountDbContext accountDbContext, MetricCacheWriteObject writeObject)
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateBatchObject(AccountDbContext accountDbContext, MetricCacheWriteObject metric, bool useCheck)
        {
            var oldMetric = metric.Response.LastSavedData;
            if (oldMetric == null)
            {
                throw new Exception("oldMetric == null");
            }

            var dbEntity = oldMetric.CreateEf();
            accountDbContext.Metrics.Attach(dbEntity);

            dbEntity.ActualDate = metric.ActualDate;
            dbEntity.BeginDate = metric.BeginDate;
            dbEntity.ComponentId = metric.ComponentId;
            dbEntity.CreateDate = metric.CreateDate;
            dbEntity.Enable = metric.Enable;
            dbEntity.DisableComment = metric.DisableComment;
            dbEntity.DisableToDate = metric.DisableToDate;
            dbEntity.IsDeleted = metric.IsDeleted;
            dbEntity.MetricTypeId = metric.MetricTypeId;
            dbEntity.ParentEnable = metric.ParentEnable;
            dbEntity.StatusDataId = metric.StatusDataId;
            dbEntity.Value = metric.Value;
            dbEntity.ConditionAlarm = metric.ConditionRed;
            dbEntity.ConditionWarning = metric.ConditionYellow;
            dbEntity.ConditionSuccess = metric.ConditionGreen;
            dbEntity.ActualTimeSecs = TimeSpanHelper.GetSeconds(metric.ActualTime);
            dbEntity.NoSignalColor = metric.NoSignalColor;
            dbEntity.ConditionElseColor = metric.ElseColor;
        }

        public override int BatchCount
        {
            get { return 100; }
        }

        protected override MetricCacheWriteObject LoadObject(AccountCacheRequest request, AccountDbContext accountDbContext)
        {
            var repository = accountDbContext.GetMetricRepository();
            var metric = repository.GetByIdOrNull(request.ObjectId);
            return MetricCacheWriteObject.Create(metric, request.AccountId);
        }
    }
}
