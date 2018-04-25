using System;
using System.Collections.Concurrent;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Caching
{
    public class UnitTestTypeCacheStorage : AccountDbCacheStorageBase<UnitTestTypeCacheResponse, IUnitTestTypeCacheReadObject, UnitTestTypeCacheWriteObject>
    {
        /// <summary>
        /// Карта актуальных типов
        /// </summary>
        protected ConcurrentDictionary<string, Guid> NameToIdMap = new ConcurrentDictionary<string, Guid>();

        protected readonly object UpdateMapLock = new object();

        private string GetMapKey(Guid accountId, string name)
        {
            return accountId + "#" + name.ToLowerInvariant();
        }

        public IUnitTestTypeCacheReadObject FindByName(Guid accountId, string name)
        {
            var key = GetMapKey(accountId, name);
            Guid unitTestTypeId = Guid.Empty;

            lock (UpdateMapLock)
            {
                if (NameToIdMap.TryGetValue(key, out unitTestTypeId))
                {
                    return Read(new AccountCacheRequest()
                    {
                        AccountId = accountId,
                        ObjectId = unitTestTypeId
                    });
                }
            }

            using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
            {
                var repository = accountDbContext.GetUnitTestTypeRepository();
                var unitTestType = repository.GetOneOrNullBySystemName(name);
                if (unitTestType != null)
                {
                    unitTestTypeId = unitTestType.Id;
                    lock (UpdateMapLock)
                    {
                        NameToIdMap.TryAdd(key, unitTestTypeId);
                    }
                    return Read(new AccountCacheRequest()
                    {
                        AccountId = accountId,
                        ObjectId = unitTestTypeId
                    });
                }
            }
            return null;
        }

        protected override void SetResponseSaved(UnitTestTypeCacheWriteObject writeObject)
        {
            base.SetResponseSaved(writeObject);

            if (writeObject.IsDeleted)
            {
                // удаляем из карты только после сохранения в БД, чтобы не было дублей
                DeleteFromMap(writeObject.AccountId, writeObject.Id, writeObject.SystemName);
            }
        }

        protected void RenameMap(Guid accountId, Guid unitTestTypeId, string oldName, string newName)
        {
            var key = GetMapKey(accountId, oldName);
            lock (UpdateMapLock)
            {
                Guid id;
                if (NameToIdMap.ContainsKey(key))
                {
                    throw new UserFriendlyException("Название типа проверки должно быть уникальным");
                }
                if (NameToIdMap.TryRemove(key, out id))
                {
                    if (id != unitTestTypeId)
                    {
                        throw new Exception("id != unitTestTypeId");
                    }
                }
                bool added = NameToIdMap.TryAdd(key, unitTestTypeId);
                if (added == false)
                {
                    throw new Exception("added == false");
                }
            }
        }

        protected void DeleteFromMap(Guid accountId, Guid unitTestTypeId, string name)
        {
            var key = GetMapKey(accountId, name);
            lock (UpdateMapLock)
            {
                Guid id;
                if (NameToIdMap.TryGetValue(key, out id))
                {
                    // удаляем по ИД, а не по имени !!!
                    if (id == unitTestTypeId)
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
            return new UserFriendlyException("Не удалось найти тип проверки с ID " + request.ObjectId);
        }

        protected override void ValidateChanges(UnitTestTypeCacheWriteObject oldObj, UnitTestTypeCacheWriteObject newObj)
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

        protected override bool HasChanges(UnitTestTypeCacheWriteObject oldObj, UnitTestTypeCacheWriteObject newObj)
        {
            return ObjectChangesHelper.HasChanges(oldObj, newObj);
        }

        protected override void AddBatchObject(AccountDbContext accountDbContext, UnitTestTypeCacheWriteObject writeObject)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObject(AccountDbContext accountDbContext, UnitTestTypeCacheWriteObject unitTestType, bool useCheck)
        {
            var oldUnitTestType = unitTestType.Response.LastSavedData;
            if (oldUnitTestType == null)
            {
                throw new Exception("oldUnitTestType == null");
            }

            var dbEntity = oldUnitTestType.CreateEf();
            accountDbContext.UnitTestTypes.Attach(dbEntity);

            dbEntity.CreateDate = unitTestType.CreateDate;
            dbEntity.IsDeleted = unitTestType.IsDeleted;
            dbEntity.CreateDate = unitTestType.CreateDate;
            dbEntity.SystemName = unitTestType.SystemName;
            dbEntity.DisplayName = unitTestType.DisplayName;
            dbEntity.ActualTimeSecs = unitTestType.ActualTimeSecs;
            dbEntity.NoSignalColor = unitTestType.NoSignalColor;
            dbEntity.IsSystem = unitTestType.IsSystem;
        }

        public override int BatchCount
        {
            get { return 100; }
        }

        protected override UnitTestTypeCacheWriteObject LoadObject(AccountCacheRequest request, AccountDbContext accountDbContext)
        {
            var repository = accountDbContext.GetUnitTestTypeRepository();
            var unitTestType = repository.GetByIdOrNull(request.ObjectId);
            return UnitTestTypeCacheWriteObject.Create(unitTestType, request.AccountId);
        }
    }
}
