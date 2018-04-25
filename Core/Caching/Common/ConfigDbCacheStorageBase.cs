using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Zidium.Core.Caching
{
    public abstract class ConfigDbCacheStorageBase<TResponse, TReadObject, TWriteObject>
        : CacheStorageBaseT<CacheRequest, TResponse, TReadObject, TWriteObject>
        where TReadObject : class, ICacheReadObject
        where TWriteObject : class, TReadObject, ICacheWriteObjectT<TResponse, TWriteObject>
        where TResponse : CacheResponse<CacheRequest, TResponse, TReadObject, TWriteObject>, new()
    {
        protected override CacheRequest GetRequest(TReadObject readObject)
        {
            if (readObject == null)
            {
                throw new ArgumentNullException("readObject");
            }
            return new CacheRequest()
            {
                ObjectId = readObject.Id
            };
        }

        protected override void AddList(List<TWriteObject> writeObjects)
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
                var saveDate = DateTime.Now;

                var batch = batchItems
                    .Select(x => x.WriteObject)
                    .ToList();

                AddBatch(batch);

                Interlocked.Increment(ref AddBatchCount);

                // обновим статистику
                foreach (var batchItem in batch)
                {
                    SetResponseSaved(batchItem);
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
                    .ToList();

                UpdateBatch(batch);

                Interlocked.Increment(ref UpdateBatchCount);

                // обновим статистику
                foreach (var batchItem in batch)
                {
                    SetResponseSaved(batchItem);
                }
            }
        }

        protected abstract void AddBatchObject(TWriteObject writeObject);

        protected void AddBatch(List<TWriteObject> cacheObjects)
        {
            if (cacheObjects == null)
            {
                throw new ArgumentNullException("cacheObjects");
            }
            if (cacheObjects.Count == 0)
            {
                return;
            }

            foreach (var cacheObject in cacheObjects)
            {
                AddBatchObject(cacheObject);
            }
        }

        protected abstract void UpdateBatchObject(TWriteObject writeObject);

        protected void UpdateBatch(List<TWriteObject> cacheObjects)
        {
            if (cacheObjects == null)
            {
                throw new ArgumentNullException("cacheObjects");
            }
            if (cacheObjects.Count == 0)
            {
                return;
            }

            foreach (var cacheObject in cacheObjects)
            {
                UpdateBatchObject(cacheObject);
            }

            // обновим статистику
            foreach (var batchItem in cacheObjects)
            {
                SetResponseSaved(batchItem);
            }

        }
    }
}
