using System;
using System.Collections.Generic;

namespace Zidium.Core.Caching
{
    // TODO Remove not implemented class
    public class AddEntitiesToAccountDbProcessor<TCacheObject, TEntity>
        where TEntity : class 
    {
        public void Add(List<TCacheObject> cacheObjects, Func<TCacheObject, TEntity> getEntity)
        {
            throw new NotImplementedException();
            /*
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
                    var dbEntity = getEntity(cacheObject);
                    if (dbEntity == null)
                    {
                        throw new Exception("dbEntity == null");
                    }
                    var entry = accountDbContext.Entry(dbEntity);
                    entry.State = EntityState.Added;
                }
                accountDbContext.SaveChanges();
            }
            */
        }

        public void Update(
            List<TCacheObject> cacheObjects,
            Func<TCacheObject, TEntity> getCurrentEntity,
            Func<TCacheObject, TEntity> getLastSavedEntity)
        {
            throw new NotImplementedException();
            /*
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
                    var lastSavedEntity = getLastSavedEntity(cacheObject);
                    if (lastSavedEntity == null)
                    {
                        throw new Exception("lastSavedEntity == null");
                    }
                    var currentEntity = getCurrentEntity(cacheObject);
                    if (currentEntity == null)
                    {
                        throw new Exception("currentEntity == null");
                    }
                    var entry = accountDbContext.Entry(lastSavedEntity);
                    entry.CurrentValues.SetValues(currentEntity);
                }
                accountDbContext.SaveChanges();
            }
            */
        }
    }
}
