using System;
using System.Collections.Concurrent;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Core.Limits;

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

        public Guid CreateMetricType(Guid accountId, string name)
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
                return GetOrCreateTypeId(accountId, name);
            }
        }

        public Guid GetOrCreateTypeId(Guid accountId, string name)
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
                using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
                {
                    var repository = accountDbContext.GetMetricTypeRepository();
                    var metricType = repository.GetOneOrNullByName(name);
                    if (metricType == null/* || metricType.IsDeleted*/) // репозиторий не возвращает удалённые
                    {
                        // Проверим лимиты
                        var limitChecker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
                        var checkResult = limitChecker.CheckMaxMetrics(accountDbContext);
                        if (!checkResult.Success)
                            throw new OverLimitException(checkResult.Message);

                        metricType = new MetricType()
                        {
                            Id = Guid.NewGuid(),
                            IsDeleted = false,
                            CreateDate = DateTime.Now,
                            SystemName = name,
                            DisplayName = name
                        };
                        repository.Add(metricType);
                        accountDbContext.SaveChanges();

                        limitChecker.RefreshMetricsCount();
                    }
                    bool success = NameToIdMap.TryAdd(key, metricType.Id);
                    if (success)
                    {
                        return metricType.Id;
                    }
                    throw new Exception("Не удалось добавить тип метрики в карту");
                }
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

        protected override void AddBatchObject(AccountDbContext accountDbContext, MetricTypeCacheWriteObject writeObject)
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateBatchObject(AccountDbContext accountDbContext, MetricTypeCacheWriteObject metricType, bool useCheck)
        {
            var oldMetricType = metricType.Response.LastSavedData;
            if (oldMetricType == null)
            {
                throw new Exception("oldMetricType == null");
            }

            var dbEntity = oldMetricType.CreateEf();
            accountDbContext.MetricTypes.Attach(dbEntity);

            dbEntity.CreateDate = metricType.CreateDate;
            dbEntity.IsDeleted = metricType.IsDeleted;
            dbEntity.CreateDate = metricType.CreateDate;
            dbEntity.SystemName = metricType.SystemName;
            dbEntity.DisplayName = metricType.DisplayName;
            dbEntity.ActualTimeSecs = TimeSpanHelper.GetSeconds(metricType.ActualTime);
            dbEntity.NoSignalColor = metricType.NoSignalColor;
            dbEntity.ConditionAlarm = metricType.ConditionRed;
            dbEntity.ConditionWarning = metricType.ConditionYellow;
            dbEntity.ConditionSuccess = metricType.ConditionGreen;
            dbEntity.ConditionElseColor = metricType.ElseColor;
        }

        public override int BatchCount
        {
            get { return 100; }
        }

        protected override MetricTypeCacheWriteObject LoadObject(AccountCacheRequest request, AccountDbContext accountDbContext)
        {
            var repository = accountDbContext.GetMetricTypeRepository();
            var metricType = repository.GetByIdOrNull(request.ObjectId);
            return MetricTypeCacheWriteObject.Create(metricType, request.AccountId);
        }
    }
}
