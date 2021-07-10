using System;
using System.Collections.Concurrent;
using System.Linq;
using Zidium.Common;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class UnitTestTypeCacheStorage : AccountDbCacheStorageBase<UnitTestTypeCacheResponse, IUnitTestTypeCacheReadObject, UnitTestTypeCacheWriteObject>
    {
        /// <summary>
        /// Карта актуальных типов
        /// </summary>
        protected ConcurrentDictionary<string, Guid> NameToIdMap = new ConcurrentDictionary<string, Guid>();

        protected readonly object UpdateMapLock = new object();

        private string GetMapKey(string name)
        {
            return name.ToLowerInvariant();
        }

        public IUnitTestTypeCacheReadObject FindByName(string name, IStorage storage)
        {
            var key = GetMapKey(name);
            Guid unitTestTypeId = Guid.Empty;

            lock (UpdateMapLock)
            {
                if (NameToIdMap.TryGetValue(key, out unitTestTypeId))
                {
                    return Read(new AccountCacheRequest()
                    {
                        ObjectId = unitTestTypeId
                    });
                }
            }

            var unitTestType = storage.UnitTestTypes.GetOneOrNullBySystemName(name);
            if (unitTestType != null)
            {
                unitTestTypeId = unitTestType.Id;
                lock (UpdateMapLock)
                {
                    NameToIdMap.TryAdd(key, unitTestTypeId);
                }
                return Read(new AccountCacheRequest()
                {
                    ObjectId = unitTestTypeId
                });
            }

            return null;
        }

        protected override void SetResponseSaved(UnitTestTypeCacheWriteObject writeObject)
        {
            base.SetResponseSaved(writeObject);

            if (writeObject.IsDeleted)
            {
                // удаляем из карты только после сохранения в БД, чтобы не было дублей
                DeleteFromMap(writeObject.Id, writeObject.SystemName);
            }
        }

        protected void RenameMap(Guid unitTestTypeId, string oldName, string newName)
        {
            var key = GetMapKey(oldName);
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

        protected void DeleteFromMap(Guid unitTestTypeId, string name)
        {
            var key = GetMapKey(name);
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
                RenameMap(newObj.Id, oldObj.SystemName, newObj.SystemName);
            }
        }

        protected override bool HasChanges(UnitTestTypeCacheWriteObject oldObj, UnitTestTypeCacheWriteObject newObj)
        {
            return ObjectChangesHelper.HasChanges(oldObj, newObj);
        }

        protected override void AddBatchObjects(IStorage storage, UnitTestTypeCacheWriteObject[] writeObjects)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateBatchObjects(IStorage storage, UnitTestTypeCacheWriteObject[] unitTestTypes, bool useCheck)
        {
            var entities = unitTestTypes.Select(unitTestType =>
            {
                var lastData = unitTestType.Response.LastSavedData;
                if (lastData == null)
                {
                    throw new Exception("unitTestType.Response.LastSavedData == null");
                }

                var entity = lastData.CreateEf();

                if (lastData.IsDeleted != unitTestType.IsDeleted)
                    entity.IsDeleted.Set(unitTestType.IsDeleted);

                if (lastData.SystemName != unitTestType.SystemName)
                    entity.SystemName.Set(unitTestType.SystemName);

                if (lastData.DisplayName != unitTestType.DisplayName)
                    entity.DisplayName.Set(unitTestType.DisplayName);

                if (lastData.ActualTimeSecs != unitTestType.ActualTimeSecs)
                    entity.ActualTimeSecs.Set(unitTestType.ActualTimeSecs);

                if (lastData.NoSignalColor != unitTestType.NoSignalColor)
                    entity.NoSignalColor.Set(unitTestType.NoSignalColor);

                return entity;
            }).ToArray();

            storage.UnitTestTypes.Update(entities);
        }

        public override int BatchCount
        {
            get { return 100; }
        }

        protected override UnitTestTypeCacheWriteObject LoadObject(AccountCacheRequest request, IStorage storage)
        {
            var unitTestType = storage.UnitTestTypes.GetOneOrNullById(request.ObjectId);
            return UnitTestTypeCacheWriteObject.Create(unitTestType);
        }
    }
}
