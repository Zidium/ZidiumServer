using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Zidium.Core.AccountsDb;
using Zidium.Core.ConfigDb;

namespace Zidium.Core.Caching
{
    public abstract class AccountDbCacheStorageBase<TResponse, TReadObject, TWriteObject>
        : CacheStorageBaseT<AccountCacheRequest, TResponse, TReadObject, TWriteObject>
        where TReadObject : class, IAccountDbCacheReadObject
        where TWriteObject: class, TReadObject, ICacheWriteObjectT<TResponse, TWriteObject> 
        where TResponse : CacheResponse<AccountCacheRequest, TResponse, TReadObject, TWriteObject>, new()
    {
        protected override AccountCacheRequest GetRequest(TReadObject cacheReadObject)
        {
            return new AccountCacheRequest()
            {
                AccountId = cacheReadObject.AccountId,
                ObjectId = cacheReadObject.Id
            };
        }

        protected abstract void AddBatchObject(AccountDbContext accountDbContext, TWriteObject writeObject);

        protected void AddBatch(Guid accountId, List<TWriteObject> cacheObjects)
        {
            if (cacheObjects == null)
            {
                throw new ArgumentNullException("cacheObjects");
            }
            if (cacheObjects.Count == 0)
            {
                return;
            }
            if (accountId == Guid.Empty)
            {
                throw new Exception("accountId == Guid.Empty");
            }
            using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
            {
                foreach (var cacheObject in cacheObjects)
                {
                    AddBatchObject(accountDbContext, cacheObject);
                }
                accountDbContext.SaveChanges();
            }
        }

        protected abstract void UpdateBatchObject(AccountDbContext accountDbContext, TWriteObject writeObject, bool useCheck);

        protected virtual void UpdateBatch(Guid accountId, List<TWriteObject> cacheObjects, bool useCheck)
        {
            if (cacheObjects == null)
            {
                throw new ArgumentNullException("cacheObjects");
            }
            if (cacheObjects.Count == 0)
            {
                return;
            }
            if (accountId == Guid.Empty)
            {
                throw new Exception("accountId == Guid.Empty");
            }
            using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
            {
                foreach (var cacheObject in cacheObjects)
                {
                    UpdateBatchObject(accountDbContext, cacheObject, useCheck);
                }
                accountDbContext.SaveChanges();
            }
        }

        public abstract int BatchCount { get; }

        protected override void AddList(List<TWriteObject> writeObjects)
        {
            var accountGroups = writeObjects.GroupBy(x => x.AccountId);
            foreach (var accountGroup in accountGroups)
            {
                var accountId = accountGroup.Key;
                int batchCount = BatchCount;

                // получим пачки
                var batchGroups = accountGroup
                    .OrderBy(x=>x.SaveOrder)
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
                        .ToList();

                    AddBatch(accountId, batch);
                    _addDataBaseCount += batch.Count;
                    Interlocked.Increment(ref AddBatchCount);

                    // обновим статистику
                    foreach (var cacheObject in batch)
                    {
                        SetResponseSaved(cacheObject);
                    }
                }
            }
        }

        protected override void UpdateList(List<TWriteObject> writeObjects)
        {
            const int batchCount = 100; //todo

            var accountGroups = writeObjects.GroupBy(x => x.AccountId);

            foreach (var accountGroup in accountGroups)
            {
                var accountId = accountGroup.Key;

                // получим пачки
                var batchGroups = accountGroup
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
                        .ToList();

                    // делаем N попыток
                    int attemps = 0;
                    bool useCheck = false;
                    while (true)
                    {
                        attemps++;
                        try
                        {
                            UpdateBatch(accountId, batch, useCheck);
                            _updateDataBaseCount += batch.Count;

                            // отправим событие
                            var saveEvent = ComponentControl.CreateComponentEvent("UpdateBatch");
                            saveEvent.SetImportance(Zidium.Api.EventImportance.Success);
                            saveEvent.SetJoinInterval(TimeSpan.FromMinutes(1));
                            saveEvent.Add();

                            break;
                        }
                        catch (Exception exception)
                        {
                            // отправим событие
                            var errorEvent = ComponentControl.CreateApplicationError("UpdateBatchError", exception);
                            errorEvent.SetImportance(Zidium.Api.EventImportance.Alarm);
                            errorEvent.SetJoinInterval(TimeSpan.FromMinutes(1));
                            errorEvent.Add();

                            useCheck = true;
                            if (attemps >= 50)
                            {
                                throw;
                            }
                            ComponentControl.Log.Error("Ошибка UpdateBatch. Попытка " + attemps, exception);
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
        }

        public override TReadObject Find(AccountCacheRequest request)
        {
            var result = base.Find(request);

            if (result == null)
                return null;

            if (result.AccountId != request.AccountId)
                return null;

            return result;
        }

        protected abstract TWriteObject LoadObject(AccountCacheRequest request, AccountDbContext accountDbContext);

        protected override TWriteObject LoadObject(AccountCacheRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (request.AccountId == Guid.Empty)
            {
                throw new Exception("request.AccountId == Guid.Empty");
            }
            using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(request.AccountId))
            {
                return LoadObject(request, accountDbContext);
            }
        }
    }
}
