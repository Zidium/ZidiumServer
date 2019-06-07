using System;
using System.Threading;

namespace Zidium.Core.Caching
{
    public abstract class CacheWriteObjectBase<TRequest, TResponse, TReadObject, TWriteObject> : ICacheWriteObjectT<TResponse, TWriteObject>, ICacheReadObject
        where TRequest : class 
        where TResponse : CacheResponse<TRequest, TResponse, TReadObject, TWriteObject>
        where TReadObject : class
        where TWriteObject : class, ICacheWriteObjectT<TResponse, TWriteObject>
    {
        public long SaveOrder { get; set; }
        public TResponse Response { get; set; }

        public Guid Id { get; set; }

        private int _dataVersion;

        public int DataVersion
        {
            get { return _dataVersion; }
        }

        public void IncrementDataVersion()
        {
            _dataVersion++;
        }

        public virtual TWriteObject GetCopy()
        {
            return MemberwiseClone() as TWriteObject;
        }

        public abstract int GetCacheSize();

        public void BeginSave()
        {
            if (Response.Unloaded)
            {
                // todo лучше проверять есть ли изменения.
                // если изменений нет, то все ОК
                // если изменения есть, то кидать исключения
                // сейчас не понятно как определить были ли внесены изменения после вызова writeObject.Unload();
                throw new Exception("Нельзя сохранять изменения выгруженного объекта");
            }
            Response.CacheStorage.BeginSaveObject(Response, this as TWriteObject);
        }

        /// <summary>
        /// Выгружает объект из кэша
        /// Могут быть потерены изменения!
        /// </summary>
        public void Unload()
        {
            Response.CacheStorage.Unload(Response.Request);
        }

        /// <summary>
        /// Ждем когда данная версия данных будет сохранена
        /// </summary>
        public void WaitSaveChanges()
        {
            WaitSaveChanges(TimeSpan.FromSeconds(30));
        }

        public void WaitSaveChanges(TimeSpan timeOut)
        {
            // если нет изменений
            if (Response.HasChanges == false)
            {
                return;
            }

            // изменения есть
            int version = _dataVersion;
            var startTime = DateTime.Now;
            while (true)
            {
                // если нет изменений
                if (Response.HasChanges == false)
                {
                    return;
                }

                // проверим сохранены ли наши изменения
                if (Response.LastSavedData != null && Response.LastSavedData.DataVersion >= version)
                {
                    return;
                }

                // проверим таймаут
                var duration = DateTime.Now - startTime;
                if (duration > timeOut)
                {
                    var exception = new Exception("Превышен таймаут ожидания сохранения изменений");
                    exception.Data.Add("Id", Response.CurrentData?.Id);
                    exception.Data.Add("LastSaveDate", Response.LastSaveDate);
                    exception.Data.Add("LastChangeDate", Response.LastChangeDate);
                    exception.Data.Add("Now", DateTime.Now);
                    throw exception;
                }
                Thread.Sleep(50);
            }
        }

        public void Dispose()
        {
            Response.Lock.Exit();
        }
    }
}
