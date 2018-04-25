using Xunit;
using Zidium.Core.Caching;
using Zidium.TestTools;

namespace Zidium.Core.Single.Tests
{
    public class AccountCacheTests
    {
        [Fact]
        public void LockRepeat()
        {
            var account = TestHelper.GetTestAccount();
            var componentObj = account.CreateTestApplicationComponent();
            var componentId = componentObj.Id;
            ComponentCacheWriteObject component0 = null;
            var cache = new AccountCache(account.Id);
            
                var component1 = cache.Components.Write(componentId);
                Assert.Equal(1, component1.Response.Lock.Count);

                var component2 = cache.Components.Write(componentId);
                Assert.Equal(2, component1.Response.Lock.Count);

                component0 = component1;

            component1.Dispose();
            component2.Dispose();
           
            Assert.False(component0.Response.Lock.IsLocked);
            Assert.Equal(0, component0.Response.Lock.Count);

            account.CheckCacheNoLocks();
        }

        [Fact]
        public void GetComponentForWriteRepeat()
        {
            var account = TestHelper.GetTestAccount();
            var componentObj = account.CreateTestApplicationComponent();
            var componentId = componentObj.Id;

            ComponentCacheWriteObject component0 = null;
            var cache = new AccountCache(account.Id);

            // проверим, что повторное получение НЕ создает новой копии
            var component1 = cache.Components.Write(componentId);
            var component2 = cache.Components.Write(componentId);
            Assert.True(ReferenceEquals(component1, component2));
            Assert.Equal(component1.DataVersion, component2.DataVersion);
            component0 = component1;

            // проверим, что обращение к кэшу напрямую тоже НЕ приведет к дублям
            var component3 = AllCaches.Components.Write(new AccountCacheRequest()
            {
                AccountId = account.Id,
                ObjectId = componentId
            });

            Assert.True(ReferenceEquals(component1, component3));
            Assert.Equal(component1.DataVersion, component3.DataVersion);

            component1.Dispose();
            component2.Dispose();
            component3.Dispose();
            account.CheckCacheNoLocks();

            // после сохранение и очистки cache, убедимся, что объект НЕ изменился

            // проверим, что повторное получение НЕ создает новой копии
            var component4 = cache.Components.Write(componentId);
            Assert.True(ReferenceEquals(component0, component4));
            Assert.Equal(component0.DataVersion, component4.DataVersion);
            component0 = component4;

            // проверим, что обращение к кэшу напрямую тоже НЕ приведет к дублям
            var component5 = AllCaches.Components.Write(new AccountCacheRequest()
            {
                AccountId = account.Id,
                ObjectId = componentId
            });

            Assert.True(ReferenceEquals(component0, component5));
            Assert.Equal(component0.DataVersion, component5.DataVersion);

            component5.DisableComment = "test comment";

            component5.BeginSave();

            component4.Dispose();
            component5.Dispose();
            account.CheckCacheNoLocks();

            // теперь получим новый объект, т.к. были сохранены изменения

            // проверим, что повторное получение НЕ создает новой копии
            var component6 = cache.Components.Write(componentId);
            Assert.False(ReferenceEquals(component0, component6));
            Assert.False(component0.DataVersion == component6.DataVersion);

            // проверим, что обращение к кэшу напрямую тоже НЕ приведет к дублям
            var component7 = AllCaches.Components.Write(new AccountCacheRequest()
            {
                AccountId = account.Id,
                ObjectId = componentId
            });

            Assert.False(ReferenceEquals(component0, component7));
            Assert.False(component0.DataVersion == component7.DataVersion);

            Assert.True(ReferenceEquals(component6, component7));
            component6.Dispose();
            component7.Dispose();
            account.CheckCacheNoLocks();
        }
    }
}
