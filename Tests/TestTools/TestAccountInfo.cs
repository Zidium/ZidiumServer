using System;
using System.Linq;
using Zidium.Api;
using Zidium.Api.XmlConfig;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Caching;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using Zidium.Storage.Ef;

namespace Zidium.TestTools
{
    public class TestAccountInfo
    {
        public Guid Id { get; set; }

        public Guid RootId { get; protected set; }

        public string SecretKey { get; set; }

        public Guid AccountDataBaseId { get; set; }

        public string SystemName { get; set; }

        public Api.AccessToken Token { get; set; }

        public string ConnectionString { get; set; }

        public void SaveAllCaches()
        {
            var request = new SaveAllCachesRequest()
            {
                Token = GetCoreLocalToken()
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var response = dispatcher.SaveAllCaches(request);
            response.Check();
        }

        public Core.Api.AccessToken GetCoreToken()
        {
            return new Core.Api.AccessToken()
            {
                AccountName = SystemName,
                IsLocalRequest = false,
                ProgramName = Token.Program,
                SecretKey = Token.SecretKey
            };
        }

        public Core.Api.AccessToken GetCoreLocalToken()
        {
            return new Core.Api.AccessToken()
            {
                AccountId = Id,
                IsLocalRequest = true,
                ProgramName = Token.Program,
                SecretKey = SystemAccountHelper.GetSystemToken().SecretKey
            };
        }

        public Config Config = new Config();

        private IClient _client;

        public IClient GetClient()
        {
            if (_client == null)
            {
                var webService = TestHelper.GetWebService(Token, SystemName);
                Config.Access.AccountName = SystemName;
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

        public Core.Api.ComponentInfo CreateTestApplicationComponent()
        {
            var componentTypeId = TestHelper.CreateRandomComponentTypeId(Id);
            var data = new GetOrCreateComponentRequestData()
            {
                SystemName = "test component systemName " + Guid.NewGuid(),
                DisplayName = "test component dispay name " + Guid.NewGuid(),
                TypeId = componentTypeId,
                ParentComponentId = RootId
            };
            var dispatcher = GetDispatcherClient();
            var response = dispatcher.GetOrCreateComponent(Id, data);
            return response.Data.Component;
        }

        public static TestAccountInfo Create(AccountInfo account)
        {
            var result = new TestAccountInfo()
            {
                Id = account.Id,
                RootId = account.RootId,
                SecretKey = account.SecretKey,
                AccountDataBaseId = account.AccountDatabaseId,
                SystemName = account.SystemName,
                Token = new Api.AccessToken()
                {
                    SecretKey = account.SecretKey
                }
            };

            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            var database = configDbServicesFactory.GetDatabaseService().GetOneById(account.AccountDatabaseId);

            result.ConnectionString = database.ConnectionString;
            return result;
        }

        internal AccountDbContext GetAccountDbContext()
        {
            return AccountDbContext.CreateFromConnectionString(null, ConnectionString);
        }

        public IMetricTypeCacheReadObject GetMetricTypeCache(Guid metricTypeId)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = Id,
                ObjectId = metricTypeId
            };
            return AllCaches.MetricTypes.Find(request);
        }

        public void CheckCacheNoLocks()
        {
            Guid accountId = Id;

            // компоненты
            var components = AllCaches.Components.GetLocked().Where(x => x.AccountId == accountId).ToList();
            if (components.Count > 0)
            {
                var first = components.First();
                throw new Exception($"Есть заблокированные компоненты: {components.Count}; first: {first.Id}");
            }

            // юнит-тесты
            var unitTests = AllCaches.UnitTests.GetLocked().Where(x => x.AccountId == accountId).ToList();
            if (unitTests.Count > 0)
            {
                var first = unitTests.First();
                throw new Exception($"Есть заблокированные юнит-тесты: {unitTests.Count}; first: {first.Id}");
            }

            // метрики
            var metrics = AllCaches.Metrics.GetLocked().Where(x => x.AccountId == accountId).ToList();
            if (metrics.Count > 0)
            {
                var first = metrics.First();
                throw new Exception($"Есть заблокированные метрики: {metrics.Count}; first: {first.Id}");
            }

            // колбаски
            var statusDatas = AllCaches.StatusDatas.GetLocked().Where(x => x.AccountId == accountId).ToList();
            if (statusDatas.Count > 0)
            {
                var first = statusDatas.First();
                throw new Exception($"Есть заблокированные колбаски: {statusDatas.Count}; first: {first.Id}");
            }

            // события
            var events = AllCaches.Events.GetLocked().Where(x => x.AccountId == accountId).ToList();
            if (events.Count > 0)
            {
                var first = events.First();
                throw new Exception($"Есть заблокированные события: {events.Count}; first: {first.Id}");
            }
        }

        public void SetComponentLogConfigIsInfoEnabled(Guid componentId, bool value)
        {
            using (var accountDbContext = GetAccountDbContext())
            {
                var componentRow = accountDbContext.Components.First(x => x.Id == componentId);
                componentRow.LogConfig.IsInfoEnabled = value;
                componentRow.LogConfig.LastUpdateDate = componentRow.LogConfig.LastUpdateDate.AddSeconds(2);
                accountDbContext.SaveChanges();
            }
        }
    }
}
