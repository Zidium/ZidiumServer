using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.Caching
{
    public class AccountCacheRepository<TResponse, TReadObject, TWriteObject>
        where TResponse : CacheResponse<AccountCacheRequest, TResponse, TReadObject, TWriteObject>
        where TReadObject : class
        where TWriteObject : class, ICacheWriteObjectT<TResponse, TWriteObject>
    {
        protected ICacheStorageT<AccountCacheRequest, TResponse, TReadObject, TWriteObject> Storage;
        protected Guid AccountId;

        public AccountCacheRepository(Guid accountId, ICacheStorageT<AccountCacheRequest, TResponse, TReadObject, TWriteObject> storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException("storage");
            }
            AccountId = accountId;
            Storage = storage;
        }

        public TReadObject Read(Guid id)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = AccountId,
                ObjectId = id
            };
            return Storage.Read(request);
        }

        public TWriteObject Write(TReadObject readObject)
        {
            if (readObject == null)
            {
                throw new ArgumentNullException("readObject");
            }
            return Storage.Write(readObject);
        }

        public TWriteObject Write(Guid id)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = AccountId,
                ObjectId = id
            };
            return Storage.Write(request);
        }

        public void Unload(Guid id)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = AccountId,
                ObjectId = id
            };
            Storage.Unload(request);
        }

        public bool ExistsInCache(Guid id)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = AccountId,
                ObjectId = id
            };
            return Storage.ExistsInCache(request);
        }

        public IEnumerable<TReadObject> GetAllLoaded()
        {
            return Storage.GetAll()
                .Where(x => x.Request.AccountId == AccountId)
                .Select(x => x.CurrentData as TReadObject)
                .Where(x => x != null);
        }
    }
}
