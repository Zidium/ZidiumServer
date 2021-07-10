using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Zidium.Api;
using Zidium.Core.Common;

namespace Zidium.Core.Caching
{
    public abstract class CacheStorageBaseT<TRequest, TResponse, TReadObject, TWriteObject> : ICacheStorageT<TRequest, TResponse, TReadObject, TWriteObject>
        where TRequest : class, ICacheRequest
        where TReadObject : class
        where TWriteObject : class, TReadObject, ICacheWriteObjectT<TResponse, TWriteObject>
        where TResponse : CacheResponse<TRequest, TResponse, TReadObject, TWriteObject>, new()
    {
        private ConcurrentDictionary<Guid, TResponse> _dictionary = new ConcurrentDictionary<Guid, TResponse>();

        // храним не response-ы, а текущую версию объекта, чтобы в правильном порядке сохранять изменения
        private Dictionary<Guid, TWriteObject> _updateChangesObjects = new Dictionary<Guid, TWriteObject>();

        protected int _maxCount;

        public int UpdateBatchCount;
        public int AddBatchCount;

        public int SaveCount;

        private long _generation;

        public long Generation { get { return _generation; } }

        public IComponentControl ComponentControl { get; set; }

        public ILogger Logger { get; set; }

        private long _updateCacheCount;

        public long UpdateCacheCount { get { return _updateCacheCount; } }

        private long _addCacheCount;

        public long AddCacheCount { get { return _addCacheCount; } }

        protected long _updateDataBaseCount;

        public long UpdateDataBaseCount { get { return _updateDataBaseCount; } }

        protected long _addDataBaseCount;

        public long AddDataBaseCount { get { return _addDataBaseCount; } }

        private LockObject _loadDataLocks = new LockObject(1000);
        private readonly object _addResponseLock = new object();

        private int _count = 0;

        private readonly object _saveLock = new object();
        private readonly object _updateChangesLock = new object();
        protected readonly object _saveBatchLock = new object();

        private SingleThreadWorker _saveChangesLoopWorker = null;
        private SingleThreadWorker _unloadDataLoopWorker = null;
        private int _beginUnloadCount;
        private int _stopUnloadCount;

        private UpdateStats _updateStats = new UpdateStats();

        protected CacheStorageBaseT()
        {
            _maxCount = 20 * 1000;
            BeginUnloadCount = 10 * 1000;
            StopUnloadCount = 5 * 1000;
            ComponentControl = new FakeComponentControl("FakeCacheControl");
            Logger = NullLogger.Instance;
            _saveChangesLoopWorker = new SingleThreadWorker(SaveChangesLoop);
            _unloadDataLoopWorker = new SingleThreadWorker(UnloadDataLoop);
        }

        public SingleThreadWorker SaveChangesLoopWorker
        {
            get { return _saveChangesLoopWorker; }
        }

        public SingleThreadWorker UnloadDataLoopWorker
        {
            get { return _unloadDataLoopWorker; }
        }

        public int Count
        {
            get { return _count; }
        }

        protected virtual void SetResponseSaved(TWriteObject writeObject)
        {
            Interlocked.Increment(ref SaveCount);

            // обновим статистику
            var response = writeObject.Response;
            response.LastSavedData = writeObject;
            response.SaveCount++;
            response.LastSaveDate = DateTime.Now;
        }

        protected abstract TWriteObject LoadObject(TRequest request);

        protected abstract TRequest GetRequest(TReadObject cacheReadObject);

        protected virtual void BeginUnload(TWriteObject obj)
        {

        }

        /// <summary>
        /// Удаляет их кэша ненужные объекты
        /// </summary>
        protected int UnloadOldData(int unloadCountMax)
        {
            int unloadedCount = 0;

            // на всякий случай попробуем запустить процесс сохранения изменений
            TryBeginSaveChangesLoop();

            var allResponses = _dictionary.Values.ToArray();

            // сортируем по полезности, первые самые безполезные
            var sortedResponses = allResponses
                .Select(x => new
                {
                    r = x,
                    getCount = x.GetCount
                })
                .OrderBy(x => x.getCount) // меньше всех использовались
                .ThenBy(x => x.r.LastGetDate) // дольше всех не использовались
                .ToList();

            foreach (var res in sortedResponses)
            {
                var response = res.r;

                // нельзя удалять из кэша объекты, которые сейчас кем то используются
                // иначе кто то сможет получить копию не сохраненного объекта
                if (response.Lock.IsLocked)
                {
                    continue;
                }

                // нельзя удалять из кэша объекты, в которых не сохранены изменения
                if (response.HasChanges)
                {
                    continue;
                }

                if (response.Unloaded)
                {
                    continue;
                }

                // попробуем заблокировать объект
                bool processed = false;
                try
                {
                    processed = response.Lock.TryEnter(TimeSpan.Zero);

                    // если удалось заблокировать
                    if (processed)
                    {
                        // еще раз проверим состояние
                        if (response.Unloaded == false
                            && response.HasChanges == false)
                        {
                            BeginUnload(response.CurrentData);
                            TResponse removedResponse = null;
                            _dictionary.TryRemove(response.Request.ObjectId, out removedResponse);
                            if (removedResponse != null)
                            {
                                removedResponse.Unloaded = true;
                                unloadedCount++;
                                if (unloadedCount >= unloadCountMax)
                                {
                                    Interlocked.Exchange(ref _count, _dictionary.Count);
                                    return unloadedCount;
                                }
                            }
                            else
                            {
                                if (response.Unloaded == false)
                                {
                                    throw new Exception("removedResponse != null");
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (processed)
                    {
                        response.Lock.Exit();
                    }
                }
            }
            Interlocked.Exchange(ref _count, _dictionary.Count);
            return unloadedCount;
        }

        protected abstract void AddList(List<TWriteObject> writeObjects);

        protected abstract void UpdateList(List<TWriteObject> writeObjects);

        protected TResponse CreateResponse(TRequest request, TWriteObject writeObject)
        {
            // создаем response
            var response = new TResponse()
            {
                Request = request,
                CurrentData = writeObject,
                LastSavedData = writeObject,
                CacheStorage = this,
                Generation = _generation
            };
            if (writeObject != null)
            {
                writeObject.Response = response;
            }

            // добавим response
            lock (_addResponseLock)
            {
                while (Count >= MaxCount)
                {
                    UnloadOldData(100);
                }
                Interlocked.Increment(ref _count);
                var ok = _dictionary.TryAdd(request.ObjectId, response);
                if (ok == false)
                {
                    throw new Exception("ok == false");
                }
            }
            return response;
        }

        protected void UnloadDataLoop()
        {
            try
            {
                // выгружаем только если превышен порог
                if (Count < BeginUnloadCount)
                {
                    return;
                }

                // удаляем лишние
                bool hasUnloaded = false;
                int doCount = 0;
                int count = Count;
                while (count > StopUnloadCount)
                {
                    doCount++;
                    int tryUnloadCount = count - StopUnloadCount;
                    int unloaded = UnloadOldData(tryUnloadCount);
                    if (unloaded > 0)
                    {
                        hasUnloaded = true;
                    }
                    count = Count;
                }

                // обновим статистику
                if (hasUnloaded)
                {
                    _generation++;
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Ошибка UnloadDataLoop");
            }
        }

        protected void TryBeginUnloadDataLoop()
        {
            _unloadDataLoopWorker.TryStart();
        }

        protected TResponse GetOrLoadResponse(
            TRequest request,
            TWriteObject wObject = null,
            bool preloaded = false)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (_unloadDataLoopWorker.IsWorking == false && Count >= BeginUnloadCount)
            {
                TryBeginUnloadDataLoop();
            }


            TResponse response = null;
            _dictionary.TryGetValue(request.ObjectId, out response);
            if (response == null)
            {
                var loadLock = _loadDataLocks.Get(request.ObjectId);
                lock (loadLock)
                {
                    _dictionary.TryGetValue(request.ObjectId, out response);
                    if (response == null)
                    {
                        // получим актуальный ответ
                        var writeObject = wObject;
                        if (preloaded == false)
                        {
                            writeObject = LoadObject(request);
                        }
                        response = CreateResponse(request, writeObject);
                    }
                }
            }

            // обновим статистику
            // блокировку не делаем, т.к. статистику можно вести приблизительную, зато будет быстрее работать
            response.LastGetDate = DateTime.Now;
            long generation = _generation;
            if (response.Generation != generation)
            {
                Interlocked.Exchange(ref response.GetCount, 0);
                response.Generation = generation;
            }
            Interlocked.Increment(ref response.GetCount);

            return response;
        }

        public void Unload(TRequest request)
        {
            TResponse response = null;
            _dictionary.TryGetValue(request.ObjectId, out response);
            if (response != null)
            {
                try
                {
                    response.Lock.Enter();
                    if (response.Unloaded)
                    {
                        return;
                    }
                    response.Unloaded = true;

                    // todo нельзя выгружать не сохраненный объект
                    _dictionary.TryRemove(request.ObjectId, out response);
                }
                finally
                {
                    response.Lock.Exit();
                }
            }
        }

        public IEnumerable<TResponse> GetAll()
        {
            return _dictionary.Values;
        }

        protected abstract Exception CreateNotFoundException(TRequest request);

        public bool ExistsInCache(TRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            return _dictionary.ContainsKey(request.ObjectId);
        }

        public bool ExistsInStorage(TRequest request)
        {
            var readData = Find(request);
            return readData != null;
        }

        public TReadObject Read(TRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            while (true)
            {
                var response = GetOrLoadResponse(request);
                if (response.CurrentData == null)
                {
                    throw CreateNotFoundException(request);
                }

                // если респонс протух, то возьмем новый
                if (response.Unloaded)
                {
                    continue;
                }

                return response.CurrentData;
            }
        }

        public virtual TReadObject Find(TRequest request)
        {
            var response = GetOrLoadResponse(request);
            return response.CurrentData;
        }

        public TWriteObject Write(TReadObject readObject)
        {
            if (readObject == null)
            {
                throw new ArgumentNullException("readObject");
            }
            var request = GetRequest(readObject);
            return Write(request);
        }

        public TWriteObject Write(TRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            while (true)
            {
                var response = GetOrLoadResponse(request);
                if (response.CurrentData == null)
                {
                    throw CreateNotFoundException(request);
                }
                response.Lock.Enter();

                // если респонс протух, то возьмем новый
                if (response.Unloaded)
                {
                    response.Lock.Exit();
                    continue;
                }

                // если это первое вхождение в блокировку, то запомним копию
                if (response.LockedData == null || response.LockedData.DataVersion == response.CurrentData.DataVersion)
                {
                    var copy = response.CurrentData.GetCopy();
                    copy.IncrementDataVersion();
                    response.LockedData = copy;
                    return copy;
                }

                // если это повторное вхождение, то вернем тот же объект что и в первый раз
                return response.LockedData;
            }
        }

        private void AddToUpdateChangesQueue(TWriteObject writeObject)
        {
            var response = writeObject.Response;
            response.LastChangeDate = DateTime.Now;
            lock (_updateChangesLock)
            {
                _updateCacheCount++;
                writeObject.SaveOrder = _updateCacheCount;

                if (_updateChangesObjects.ContainsKey(response.Request.ObjectId))
                {
                    _updateChangesObjects[response.Request.ObjectId] = writeObject;
                }
                else
                {
                    _updateChangesObjects.Add(response.Request.ObjectId, writeObject);
                }

                response.CurrentData = writeObject;
            }
            TryBeginSaveChangesLoop();
        }

        protected abstract void ValidateChanges(TWriteObject oldObj, TWriteObject newObj);

        public bool BeginSaveObject(TResponse response, TWriteObject newObj)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            bool hasChanges = false;
            try
            {
                if (newObj != null)
                {
                    var oldObj = response.CurrentData;

                    // проверяем есть ли изменения
                    hasChanges = HasChanges(oldObj, newObj);

                    // проверим изменения
                    if (hasChanges)
                    {
                        ValidateChanges(oldObj, newObj);
                    }
                }
            }
            finally
            {
                // обновим данные в секции finally, чтобы никто не смог оборвать поток
                if (hasChanges)
                {
                    AddToUpdateChangesQueue(newObj);
                }
            }
            return hasChanges;
        }

        public int GetChangedCount()
        {
            return _dictionary.Count(x => x.Value.HasChanges);
        }

        public DateTime GetLastSaveChangesDate()
        {
            return LastSaveChangesDate;
        }

        public long GetSize()
        {
            return _dictionary.Sum(x => (long)x.Value.GetSize());
        }

        public List<TWriteObject> GetChanged()
        {
            return _dictionary.Values
                .Where(x => x.HasChanges)
                .Select(x => x.CurrentData)
                .ToList();
        }

        public List<TWriteObject> GetLocked()
        {
            return _dictionary.Values
                .Where(x => x.Lock.IsLocked)
                .Select(x => x.CurrentData)
                .ToList();
        }

        protected abstract bool HasChanges(TWriteObject oldObj, TWriteObject newObj);

        protected DateTime LastSaveChangesDate;

        public int SaveChanges()
        {
            lock (_saveLock)
            {
                LastSaveChangesDate = DateTime.Now;
                Dictionary<Guid, TWriteObject> oldUpdateChangesObjects = null;
                lock (_updateChangesLock)
                {
                    oldUpdateChangesObjects = _updateChangesObjects;
                    _updateChangesObjects = new Dictionary<Guid, TWriteObject>();
                }
                if (oldUpdateChangesObjects.Count > 0)
                {
                    var timer = new Stopwatch();
                    timer.Start();
                    Logger.LogDebug("Начинаем обновлять: " + oldUpdateChangesObjects.Count);

                    // выгруженные объекты сохранять НЕ нужно,
                    // потому что их могли изменить синхронно
                    var responses = oldUpdateChangesObjects.Values
                        .Where(x => !x.Response.Unloaded)
                        .ToList();

                    UpdateList(responses);
                    timer.Stop();
                    var count = oldUpdateChangesObjects.Count;
                    var sec = Math.Round(timer.Elapsed.TotalSeconds, 1);
                    var speed = (int)(count / timer.Elapsed.TotalSeconds);
                    Logger.LogInformation("Завершили обновлять " + count + " за " + sec + " сек, скорость = " + speed + " в сек");

                    // сохраняем статистику по максимальному времени и количеству в очереди
                    lock (_updateStats)
                    {
                        if (count > _updateStats.MaxCount)
                            _updateStats.MaxCount = count;

                        if (sec > _updateStats.MaxDuration)
                            _updateStats.MaxDuration = sec;
                    }
                }

                return oldUpdateChangesObjects.Count;
            }
        }

        protected void TryBeginSaveChangesLoop()
        {
            _saveChangesLoopWorker.TryStart();
        }

        private ExceptionTempInfo _lastSaveException;

        public ExceptionTempInfo LastSaveException
        {
            get { return _lastSaveException; }
        }

        /// <summary>
        /// Сохраняет изменения в бесконечном цикле пока есть хоть 1 изменение
        /// </summary>
        protected void SaveChangesLoop()
        {
            try
            {
                while (true)
                {
                    int count = SaveChanges();
                    if (count == 0)
                    {
                        return;
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception exception)
            {
                _lastSaveException = new ExceptionTempInfo(exception);
                Logger.LogError(exception, "Ошибка SaveChangesLoop");
            }
        }

        public int MaxCount
        {
            get { return _maxCount; }
            set { _maxCount = value; }
        }

        public int BeginUnloadCount
        {
            get { return _beginUnloadCount; }
            set { _beginUnloadCount = value; }
        }

        public int StopUnloadCount
        {
            get { return _stopUnloadCount; }
            set { _stopUnloadCount = value; }
        }

        public TReadObject AddOrGet(TWriteObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            var request = GetRequest(obj);
            var response = GetOrLoadResponse(request, obj, true);
            return response.CurrentData;
        }

        public UpdateStats GetUpdateStatsAndReset()
        {
            lock (_updateStats)
            {
                var result = _updateStats;
                _updateStats = new UpdateStats();
                return result;
            }
        }

        public class UpdateStats
        {
            public int MaxCount;
            public double MaxDuration;
        }
    }
}
