using System;
using System.Linq;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Api.XmlConfig;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Caching;
using Zidium.Core.Common.Helpers;
using Zidium.Storage.Ef;

namespace Zidium.TestTools
{
    // TODO Refactor to static class?
    public class TestAccountInfo
    {
        public string SecretKey { get; set; }

        public AccessTokenDto Token { get; set; }

        public string ConnectionString { get; set; }

        public void SaveAllCaches()
        {
            var request = new SaveAllCachesRequest()
            {
                Token = Token
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var response = dispatcher.SaveAllCaches(request);
            response.Check();
        }

        public Config Config = new Config();

        private IClient _client;

        public IClient GetClient()
        {
            if (_client == null)
            {
                var webService = TestHelper.GetWebService(Token);
                Config.Access.SecretKey = SecretKey;
                Config.Logs.AutoCreateEvents.Disable = true; // To avoid side effects
                _client = new Client(webService, Config);
            }
            return _client;
        }

        public IComponentControl CreateRandomComponentControl(IComponentControl parent = null)
        {
            var client = GetClient();
            var parentControl = parent ?? client.GetRootComponentControl();
            var message = TestHelper.GetRandomGetOrCreateComponentData(client);
            var newComponent = parentControl.GetOrCreateChildComponentControl(message);
            return newComponent;
        }

        public DispatcherClient GetDispatcherClient()
        {
            return new DispatcherClient("Tests");
        }

        public ComponentDto CreateTestApplicationComponent()
        {
            var dispatcher = GetDispatcherClient();
            var root = dispatcher.GetRoot().GetDataAndCheck();

            var componentTypeId = TestHelper.CreateRandomComponentTypeId();
            var data = new GetOrCreateComponentRequestDataDto()
            {
                SystemName = "test component systemName " + Guid.NewGuid(),
                DisplayName = "test component dispay name " + Guid.NewGuid(),
                TypeId = componentTypeId,
                ParentComponentId = root.Id
            };

            var response = dispatcher.GetOrCreateComponent(data);
            return response.GetDataAndCheck().Component;
        }

        public static TestAccountInfo Create()
        {
            var token = SystemAccountHelper.GetApiToken();

            var result = new TestAccountInfo()
            {
                SecretKey = token.SecretKey,
                Token = token
            };

            var databaseConfiguration = DependencyInjection.GetServicePersistent<IDatabaseConfiguration>();
            result.ConnectionString = databaseConfiguration.ConnectionString;

            return result;
        }

        internal AccountDbContext GetDbContext()
        {
            return TestHelper.GetDbContext();
        }

        public IMetricTypeCacheReadObject GetMetricTypeCache(Guid metricTypeId)
        {
            var request = new AccountCacheRequest()
            {
                ObjectId = metricTypeId
            };
            return AllCaches.MetricTypes.Find(request);
        }

        public void CheckCacheNoLocks()
        {
            // компоненты
            var components = AllCaches.Components.GetLocked();
            if (components.Count > 0)
            {
                var first = components.First();
                throw new Exception($"Есть заблокированные компоненты: {components.Count}; first: {first.Id}");
            }

            // юнит-тесты
            var unitTests = AllCaches.UnitTests.GetLocked();
            if (unitTests.Count > 0)
            {
                var first = unitTests.First();
                throw new Exception($"Есть заблокированные юнит-тесты: {unitTests.Count}; first: {first.Id}");
            }

            // метрики
            var metrics = AllCaches.Metrics.GetLocked();
            if (metrics.Count > 0)
            {
                var first = metrics.First();
                throw new Exception($"Есть заблокированные метрики: {metrics.Count}; first: {first.Id}");
            }

            // колбаски
            var statusDatas = AllCaches.StatusDatas.GetLocked();
            if (statusDatas.Count > 0)
            {
                var first = statusDatas.First();
                throw new Exception($"Есть заблокированные колбаски: {statusDatas.Count}; first: {first.Id}");
            }

            // события
            var events = AllCaches.Events.GetLocked();
            if (events.Count > 0)
            {
                var first = events.First();
                throw new Exception($"Есть заблокированные события: {events.Count}; first: {first.Id}");
            }
        }

        public void SetComponentLogConfigIsInfoEnabled(Guid componentId, bool value)
        {
            using (var accountDbContext = GetDbContext())
            {
                var componentRow = accountDbContext.Components.First(x => x.Id == componentId);
                componentRow.LogConfig.IsInfoEnabled = value;
                componentRow.LogConfig.LastUpdateDate = componentRow.LogConfig.LastUpdateDate.AddSeconds(2);
                accountDbContext.SaveChanges();
            }
        }

        public Guid RootId
        {
            get
            {
                return GetDispatcherClient().GetRoot().GetDataAndCheck().Id;
            }
        }
    }
}
