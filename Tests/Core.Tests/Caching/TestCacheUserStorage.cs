using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Zidium.Core.Caching;

namespace Zidium.Core.Tests.Caching
{
    class TestCacheUserStorage : CacheStorageBaseT<CacheRequest, TestCacheUserResponse, ITestCacheUserReadObject, TestCacheUserWriteObject>
    {
        private ConcurrentDictionary<Guid, TestCacheUserWriteObject> _allUsers = new ConcurrentDictionary<Guid, TestCacheUserWriteObject>();

        public TimeSpan LoadTime = TimeSpan.Zero;

        public TimeSpan SaveTime = TimeSpan.Zero;

        protected override void BeginUnload(TestCacheUserWriteObject obj)
        {
        }

        protected override void AddList(List<TestCacheUserWriteObject> writeObjects)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateList(List<TestCacheUserWriteObject> users)
        {
            foreach (var user in users)
            {
                // при сохранении обновляем только поле Money
                var dbUser = _allUsers[user.Id];
                dbUser.Money = user.Money;
                dbUser.Response = user.Response;
                dbUser.SaveCount++;
                
                SetResponseSaved(user);
            }
            if (SaveTime > TimeSpan.Zero)
            {
                Thread.Sleep(SaveTime);
            }
        }

        protected override Exception CreateNotFoundException(CacheRequest request)
        {
            throw new Exception("Не удалось найти юзера с ИД " + request.ObjectId);
        }

        protected override TestCacheUserWriteObject LoadObject(CacheRequest request)
        {
            if (LoadTime > TimeSpan.Zero)
            {
                Thread.Sleep(LoadTime);
            }

            var user = _allUsers[request.ObjectId];
            user.LoadCount++;
            return user.GetCopy();
        }

        protected override CacheRequest GetRequest(ITestCacheUserReadObject cacheReadObject)
        {
            if (cacheReadObject == null)
            {
                throw new ArgumentNullException("cacheReadObject");
            }
            return new CacheRequest()
            {
                ObjectId = cacheReadObject.Id
            };
        }

        public void CreateUsers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var user = new TestCacheUserWriteObject()
                {
                    Id = Guid.NewGuid(),
                    Name = "user " + i
                };
                _allUsers.TryAdd(user.Id, user);
            }
        }

        public List<TestCacheUserWriteObject> GetAllUsers()
        {
            return _allUsers.Values.ToList();
        } 

        protected override void ValidateChanges(TestCacheUserWriteObject oldObj, TestCacheUserWriteObject newObj)
        {
        }

        protected override bool HasChanges(TestCacheUserWriteObject oldObj, TestCacheUserWriteObject newObj)
        {
            return ObjectChangesHelper.HasChanges(oldObj, newObj);
        }

        public TestCacheUserStorage(int maxCount)
        {
            _maxCount = maxCount;
        }
    }
}
