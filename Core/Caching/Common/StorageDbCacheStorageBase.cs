﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Zidium.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Caching
{
    public abstract class StorageDbCacheStorageBase<TResponse, TReadObject, TWriteObject>
        : CacheStorageBaseT<AccountCacheRequest, TResponse, TReadObject, TWriteObject>
        where TReadObject : class, IAccountDbCacheReadObject
        where TWriteObject : class, TReadObject, ICacheWriteObjectT<TResponse, TWriteObject>
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

        protected abstract void AddBatchObject(AccountDbContext accountDbContext, TWriteObject writeObject, bool checkAdd);

        protected void AddBatch(Guid accountId, List<TWriteObject> cacheObjects, bool checkAdd)
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

            var batchTimer = new Stopwatch();
            batchTimer.Start();

            using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
            {
                accountDbContext.Configuration.AutoDetectChangesEnabled = false;
                accountDbContext.Configuration.ValidateOnSaveEnabled = false;

                foreach (var cacheObject in cacheObjects)
                {
                    AddBatchObject(accountDbContext, cacheObject, checkAdd);
                }

                accountDbContext.ChangeTracker.DetectChanges();

                var saveTimer = new Stopwatch();
                saveTimer.Start();

                //storageDbContext.Database.ExecuteSqlCommand("SELECT 'cache-add-batch-begin'");
                accountDbContext.SaveChanges();
                //storageDbContext.Database.ExecuteSqlCommand("SELECT 'cache-add-batch-end'");

                saveTimer.Stop();
                batchTimer.Stop();

                ComponentControl.Log.Debug("AddBatch " + cacheObjects.Count + " штук за " +
                    (int)batchTimer.ElapsedMilliseconds + " мс / SaveChanges за " +
                    (int)saveTimer.ElapsedMilliseconds + " мс");

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

            var batchTimer = new Stopwatch();
            batchTimer.Start();

            using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
            {
                accountDbContext.Configuration.ValidateOnSaveEnabled = false;
                accountDbContext.Configuration.AutoDetectChangesEnabled = false;

                Exception lastException = null;
                foreach (var cacheObject in cacheObjects)
                {
                    try
                    {
                        UpdateBatchObject(accountDbContext, cacheObject, useCheck);
                    }
                    catch (Exception exception)
                    {
                        // Запомним ошибку, чтобы вызвать в конце
                        lastException = exception;
                    }
                }

                accountDbContext.ChangeTracker.DetectChanges();

                var saveTimer = new Stopwatch();
                saveTimer.Start();

                // accountDbContext.Database.ExecuteSqlCommand("SELECT 'cache-update-batch-begin'");
                accountDbContext.SaveChanges();
                // accountDbContext.Database.ExecuteSqlCommand("SELECT 'cache-update-batch-end'");

                saveTimer.Stop();
                batchTimer.Stop();

                ComponentControl.Log.Debug("UpdateBatch " + cacheObjects.Count + " штук за " +
                    (int)batchTimer.ElapsedMilliseconds + " мс / SaveChanges за " +
                    (int)saveTimer.ElapsedMilliseconds + " мс, AccountId = " + accountId);

                // Сохраняем что удастся, но ошибку всё равно вызываем
                if (lastException != null)
                    throw lastException;
            }
        }

        protected override void AddList(List<TWriteObject> writeObjects)
        {
            var accountGroups = writeObjects.GroupBy(x => x.AccountId);
            foreach (var accountGroup in accountGroups)
            {
                var accountId = accountGroup.Key;

                int batchCount = 100;

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

                    var batch = batchItems
                        .Select(x => x.WriteObject)
                        .ToList();

                    // делаем N попыток
                    int attemps = 0;
                    bool chackAdd = false; // не проверять существование объектов перед вставкой
                    while (true)
                    {
                        attemps++;
                        try
                        {
                            AddBatch(accountId, batch, chackAdd);
                            _addDataBaseCount += batch.Count;
                            break;
                        }
                        catch (Exception exception)
                        {
                            // отправим событие
                            var errorEvent = ComponentControl.CreateApplicationError("AddBatchError", exception);
                            errorEvent.SetImportance(EventImportance.Warning);
                            errorEvent.Add();

                            if (attemps >= 100)
                            {
                                throw;
                            }
                            chackAdd = true;
                            ComponentControl.Log.Warning("Ошибка AddBatch. Попытка " + attemps, exception);
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                        }
                    }

                    Interlocked.Increment(ref AddBatchCount);

                    // обновим статистику
                    foreach (var batchItem in batch)
                    {
                        SetResponseSaved(batchItem);
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
                    lock (_saveBatchLock)
                    {
                        var batchItems = batchGroup.ToList();

                        // сохраним пачку
                        var batch = batchItems
                            .Select(x => x.WriteObject)
                            .ToList();

                        // отфильтруем неактуальные изменения
                        // изменения уже могли быть сохранены синхронно
                        batch = batch
                            .Where(x => x.Response.LastSavedData == null || x.Response.LastSavedData.DataVersion < x.DataVersion)
                            .ToList();

                        if (batch.Count == 0)
                        {
                            continue;
                        }

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
                                break;
                            }
                            catch (Exception exception)
                            {
                                // отправим событие
                                var errorEvent = ComponentControl.CreateApplicationError("UpdateBatchError", exception);
                                errorEvent.SetImportance(EventImportance.Warning);
                                errorEvent.Add();

                                useCheck = true;
                                if (attemps >= 100)
                                {
                                    throw;
                                }
                                ComponentControl.Log.Warning("Ошибка UpdateBatch. Попытка " + attemps, exception);
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
