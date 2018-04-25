using System;

namespace Zidium.Core.Caching
{
    public class CacheResponse<TRequest, TResponse, TReadObject, TWriteObject>
        //where TRequest 
        where TResponse : CacheResponse<TRequest, TResponse, TReadObject, TWriteObject>
        where TReadObject: class
        where TWriteObject : class, ICacheWriteObjectT<TResponse, TWriteObject> 

    {
        public CacheResponse()
        {
            _cacheLock = new CacheLock(this, TimeSpan.FromSeconds(30));
        }

        public DateTime LastGetDate { get; set; }

        public int GetCount;

        public int SaveCount { get; set; }

        public DateTime? LastSaveDate { get; set; }

        public DateTime? LastChangeDate { get; set; }

        public long Generation;

        public bool HasChanges
        {
            get
            {
                var currentData = CurrentData;
                if (currentData == null)
                {
                    return false;
                }
                var savedVersion = LastSavedData;
                if (savedVersion == null)
                {
                    return true;
                }
                return savedVersion.DataVersion != currentData.DataVersion;
            }
        }

        /// <summary>
        /// Версия объекта, которая сохранена в БД
        /// </summary>
        public TWriteObject LastSavedData { get; set; }

        /// <summary>
        /// Текущая версия, которую все получают их кэша
        /// Может иметь не сохраненнеы изменения в БД
        /// </summary>
        public TWriteObject CurrentData { get; set; }

        /// <summary>
        /// Версия объекта, которую в настоящий момент изменяют
        /// </summary>
        public TWriteObject LockedData { get; set; }

        public TRequest Request { get; set; }

        public bool Unloaded { get; set; }

        public ICacheStorageT<TRequest, TResponse, TReadObject, TWriteObject> CacheStorage { get; set; }

        private readonly CacheLock _cacheLock;

        public CacheLock Lock
        {
            get { return _cacheLock; }
        }

        public int GetSize()
        {
            int size = 1 // IsNewEntity
                       + 8 // LastGetDate
                       + 4 // GetCount
                       + 4 // SaveCount
                       + 8 // LastSaveDate
                       + 8 // LastChangeDate
                       + 8 // Generation
                       + 8 // _lockObj
                       + 1 // Unloaded
                       + 32 // Request
                       + 8 // LastSavedData
                       + 8; // CurrentData

            if (CurrentData != null)
            {
                size += CurrentData.GetCacheSize();
            }
            if (LastSavedData != null && ReferenceEquals(LastSavedData, CurrentData) == false)
            {
                size += LastSavedData.GetCacheSize();
            }
            return size;
        }
    }
}
