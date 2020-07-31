using System;
using System.Collections.Concurrent;
using System.Linq;
using Zidium.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.Limits;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class MetricTypeCacheStorage : AccountDbCacheStorageBase<MetricTypeCacheResponse, IMetricTypeCacheReadObject, MetricTypeCacheWriteObject>
    {
        /// <summary>
        /// Карта актуальных типов
        /// </summary>
        protected ConcurrentDictionary<string, Guid> NameToIdMap = new ConcurrentDictionary<string, Guid>();

        private readonly object UpdateMapLock = new object();

        private string GetMapKey(Guid accountId, string name)
        {
            return accountId + "#" + name.ToLowerInvariant();
        }

        public Guid CreateMetricType(Guid accountId, string name, IStorage storage)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            var key = GetMapKey(accountId, name);
            Guid id;
            lock (UpdateMapLock)
            {
                // проверим, что такого типа метрики нет
                if (NameToIdMap.TryGetValue(key, out id))
                {
                    throw new UserFriendlyException("Тип метрики с именем " + name + " уже есть");
                }
                return GetOrCreateTypeId(accountId, name, storage);
            }
        }

        public Guid GetOrCreateTypeId(Guid accountId, string name, IStorage storage)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            var key = GetMapKey(accountId, name);
            Guid id;
            if (NameToIdMap.TryGetValue(key, out id))
            {
                return id;
            }
            lock (UpdateMapLock)
            {
                if (NameToIdMap.TryGetValue(key, out id))
                {
                    return id;
                }

                Guid metricTypeId;
                var metricType = storage.MetricTypes.GetOneOrNullBySystemName(name);
                if (metricType == null)
                {
                    // Проверим лимиты
                    var limitChecker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
                    var checkResult = limitChecker.CheckMaxMetrics(storage);
                    if (!checkResult.Success)
                        throw new OverLimitException(checkResult.Message);

                    var metricTypeForAdd = new MetricTypeForAdd()
                    {
                        Id = Guid.NewGuid(),
                        IsDeleted = false,
                        CreateDate = DateTime.Now,
                        SystemName = name,
                        DisplayName = name
                    };
                    storage.MetricTypes.Add(metricTypeForAdd);
                    metricTypeId = metricTypeForAdd.Id;

                    limitChecker.RefreshMetricsCount();
                }
                else
                {
                    metricTypeId = metricType.Id;
                }

                var success = NameToIdMap.TryAdd(key, metricTypeId);
                if (success)
                {
                    return metricTypeId;
                }
                throw new Exception("Не удалось добавить тип метрики в карту");
            }
        }

        protected override void SetResponseSaved(MetricTypeCacheWriteObject writeObject)
        {
            base.SetResponseSaved(writeObject);

            if (writeObject.IsDeleted)
            {
                // удаляем из карты только после сохранения в БД, чтобы не было дублей
                DeleteFromMap(writeObject.AccountId, writeObject.Id, writeObject.SystemName);
            }
        }

        protected void RenameMap(Guid accountId, Guid metricTypeId, string oldName, string newName)
        {
            var key = GetMapKey(accountId, oldName);
            lock (UpdateMapLock)
            {
                Guid id;
                if (NameToIdMap.ContainsKey(key))
                {
                    throw new UserFriendlyException("Название типа метрики должно быть уникальным");
                }
                if (NameToIdMap.TryRemove(key, out id))
                {
                    if (id != metricTypeId)
                    {
                        throw new Exception("id != metricTypeId");
                    }
                }
                bool added = NameToIdMap.TryAdd(key, metricTypeId);
                if (added == false)
                {
                    throw new Exception("added == false");
                }
            }
        }

        protected void DeleteFromMap(Guid accountId, Guid metricTypeId, string name)
        {
            var key = GetMapKey(accountId, name);
            lock (UpdateMapLock)
            {
                Guid id;
                if (NameToIdMap.TryGetValue(key, out id))
                {
                    // удаляем по ИД, а не по имени !!!
                    if (id == metricTypeId)
                    {

                        // удаление в БД гарантирует SetResponseSaved

                        // сначала удалим в БД, чтобы гарантировать, что в БД не будет 2 типа с одинаковым именем
                        // иначе после удаления из кэша можно упеть создать новое, а старое не успеть удалить
                        //var dataBase = AllChaches.DataBases.GetAccountDataBaseByAccountId(accountId);
                        //using (var accountDbContext = AccountDbContext.CreateFromConnectionString(dataBase.ConnectionString))
                        //{
                        //    var repository = accountDbContext.GetMetricTypeRepository();
                        //    var metricType = repository.GetOneOrNullByName(accountId, name);
                        //    if (metricType != null && metricType.IsDeleted==false)
                        //    {
                        //        metricType.IsDeleted = true;
                        //        accountDbContext.SaveChanges();
                        //    }
                        //}

                        NameToIdMap.TryRemove(key, out id);
                    }
                }
            }
        }

        protected override Exception CreateNotFoundException(AccountCacheRequest request)
        {
            return new UserFriendlyException("Не удалось найти тип метрики с ID " + request.ObjectId);
        }

        protected override void ValidateChanges(MetricTypeCacheWriteObject oldObj, MetricTypeCacheWriteObject newObj)
        {
            if (newObj == null)
            {
                throw new ArgumentNullException("newObj");
            }
            if (oldObj == null)
            {
                throw new ArgumentNullException("oldObj");
            }
            if (newObj.SystemName != oldObj.SystemName)
            {
                RenameMap(newObj.AccountId, newObj.Id, oldObj.SystemName, newObj.SystemName);
            }
        }

        protected override bool HasChanges(MetricTypeCacheWriteObject oldObj, MetricTypeCacheWriteObject newObj)
        {
            return ObjectChangesHelper.HasChanges(oldObj, newObj);
        }

        protected override void AddBatchObjects(IStorage storage, MetricTypeCacheWriteObject[] writeObjects)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObjects(IStorage storage, MetricTypeCacheWriteObject[] metricTypes, bool useCheck)
        {
            var entities = metricTypes.Select(metricType =>
            {
                var lastData = metricType.Response.LastSavedData;
                if (lastData == null)
                {
                    throw new Exception("oldMetricType == null");
                }

                var entity = lastData.CreateEf();

                if (lastData.IsDeleted != metricType.IsDeleted)
                    entity.IsDeleted.Set(metricType.IsDeleted);

                if (lastData.SystemName != metricType.SystemName)
                    entity.SystemName.Set(metricType.SystemName);

                if (lastData.DisplayName != metricType.DisplayName)
                    entity.DisplayName.Set(metricType.DisplayName);

                if (lastData.ActualTime != metricType.ActualTime)
                    entity.ActualTimeSecs.Set(TimeSpanHelper.GetSeconds(metricType.ActualTime));

                if (lastData.NoSignalColor != metricType.NoSignalColor)
                    entity.NoSignalColor.Set(metricType.NoSignalColor);

                if (lastData.ConditionRed != metricType.ConditionRed)
                    entity.ConditionAlarm.Set(metricType.ConditionRed);

                if (lastData.ConditionYellow != metricType.ConditionYellow)
                    entity.ConditionWarning.Set(metricType.ConditionYellow);

                if (lastData.ConditionGreen != metricType.ConditionGreen)
                    entity.ConditionSuccess.Set(metricType.ConditionGreen);

                if (lastData.ElseColor != metricType.ElseColor)
                    entity.ConditionElseColor.Set(metricType.ElseColor);

                return entity;
            }).ToArray();

            storage.MetricTypes.Update(entities);
        }

        public override int BatchCount
        {
            get { return 100; }
        }

        protected override MetricTypeCacheWriteObject LoadObject(AccountCacheRequest request, IStorage storage)
        {
            var metricType = storage.MetricTypes.GetOneOrNullById(request.ObjectId);
            return MetricTypeCacheWriteObject.Create(metricType, request.AccountId);
        }
    }
}
