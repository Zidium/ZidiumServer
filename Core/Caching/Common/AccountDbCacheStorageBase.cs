using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public abstract class AccountDbCacheStorageBase<TResponse, TReadObject, TWriteObject>
        : CacheStorageBaseT<AccountCacheRequest, TResponse, TReadObject, TWriteObject>
        where TReadObject : class, IAccountDbCacheReadObject
        where TWriteObject : class, TReadObject, ICacheWriteObjectT<TResponse, TWriteObject>
        where TResponse : CacheResponse<AccountCacheRequest, TResponse, TReadObject, TWriteObject>, new()
    {
        protected AccountDbCacheStorageBase()
        {
            AccountStorageFactory = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>();
        }

        protected readonly IDefaultStorageFactory AccountStorageFactory;

        protected override AccountCacheRequest GetRequest(TReadObject cacheReadObject)
        {
            return new AccountCacheRequest()
            {
                ObjectId = cacheReadObject.Id
            };
        }

        protected abstract void AddBatchObjects(IStorage storage, TWriteObject[] writeObjects);

        protected void AddBatch(TWriteObject[] cacheObjects)
        {
            if (cacheObjects == null)
            {
                throw new ArgumentNullException("cacheObjects");
            }
            if (cacheObjects.Length == 0)
            {
                return;
            }

            var storage = AccountStorageFactory.GetStorage();
            AddBatchObjects(storage, cacheObjects);
        }

        protected abstract void UpdateBatchObjects(IStorage storage, TWriteObject[] writeObjects, bool useCheck);

        protected virtual void UpdateBatch(TWriteObject[] cacheObjects, bool useCheck)
        {
            if (cacheObjects == null)
            {
                throw new ArgumentNullException("cacheObjects");
            }
            if (cacheObjects.Length == 0)
            {
                return;
            }

            var storage = AccountStorageFactory.GetStorage();
            UpdateBatchObjects(storage, cacheObjects, useCheck);
        }

        public abstract int BatchCount { get; }

        protected override void AddList(List<TWriteObject> writeObjects)
        {
            int batchCount = BatchCount;

            // получим пачки
            var batchGroups = writeObjects
                .OrderBy(x => x.SaveOrder)
                .Select((x, index) => new
                {
                    Batch = index / batchCount,
                    WriteObject = x
                })
                .GroupBy(x => x.Batch);

            foreach (var batchGroup in batchGroups)
            {
                var batchItems = batchGroup.ToList();

                // сохраним пачку
                var saveDate = DateTime.Now;

                var batch = batchItems
                    .Select(x => x.WriteObject)
                    .ToArray();

                AddBatch(batch);
                _addDataBaseCount += batch.Length;
                Interlocked.Increment(ref AddBatchCount);

                // обновим статистику
                foreach (var cacheObject in batch)
                {
                    SetResponseSaved(cacheObject);
                }
            }
        }

        protected override void UpdateList(List<TWriteObject> writeObjects)
        {
            const int batchCount = 100;

            // получим пачки
            var batchGroups = writeObjects
                .OrderBy(x => x.SaveOrder)
                .Select((x, index) => new
                {
                    Batch = index / batchCount,
                    WriteObject = x
                })
                .GroupBy(x => x.Batch);

            foreach (var batchGroup in batchGroups)
            {
                var batchItems = batchGroup.ToList();

                // сохраним пачку
                var batch = batchItems
                    .Select(x => x.WriteObject)
                    .ToArray();

                // делаем N попыток
                int attemps = 0;
                bool useCheck = false;
                while (true)
                {
                    attemps++;
                    try
                    {
                        UpdateBatch(batch, useCheck);
                        _updateDataBaseCount += batch.Length;
                        break;
                    }
                    catch (Exception exception)
                    {
                        Logger.LogWarning(exception, "Ошибка UpdateBatch. Попытка " + attemps);

                        useCheck = true;
                        if (attemps >= 50)
                        {
                            throw;
                        }

                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
                }

                Interlocked.Increment(ref UpdateBatchCount);

                // обновим статистику
                foreach (var batchItem in batch)
                {
                    SetResponseSaved(batchItem);
                }
            }
        }

        public override TReadObject Find(AccountCacheRequest request)
        {
            var result = base.Find(request);

            if (result == null)
                return null;

            return result;
        }

        protected abstract TWriteObject LoadObject(AccountCacheRequest request, IStorage storage);

        protected override TWriteObject LoadObject(AccountCacheRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var storage = AccountStorageFactory.GetStorage();
            return LoadObject(request, storage);
        }
    }
}
