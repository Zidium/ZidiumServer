using System.Collections.Generic;

namespace Zidium.Core.Caching
{
    public interface ICacheStorageT<TRequest, TResponse, TReadObject, TWriteObject> : ICacheStorage
        where TReadObject : class
        where TWriteObject : class, ICacheWriteObjectT<TResponse, TWriteObject>
        where TResponse : CacheResponse<TRequest, TResponse, TReadObject, TWriteObject>
    {
        TReadObject AddOrGet(TWriteObject obj);

        void Unload(TRequest request);

        IEnumerable<TResponse> GetAll();

        TReadObject Find(TRequest request);

        bool ExistsInCache(TRequest request);

        bool ExistsInStorage(TRequest request);

        TReadObject Read(TRequest request);

        TWriteObject Write(TRequest request);

        TWriteObject Write(TReadObject readObject);

        bool BeginSaveObject(TResponse response, TWriteObject newObj);
    }
}
