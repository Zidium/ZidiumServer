using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using ApiAdapter;
using Zidium.Api;
using Zidium.Api.Common;
using Zidium.Api.Dto;
using Zidium.Api.Others;
using Zidium.Api.XmlConfig;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.Storage.Ef;

namespace Zidium.TestTools
{
    /// <summary>
    /// Хелпер для юнит-тестов
    /// </summary>
    public class TestHelper
    {
        public static bool CanRunApiOnIisExpress = string.Equals(ConfigurationManager.AppSettings["AutorunIisExpress"], "true", StringComparison.OrdinalIgnoreCase);

        public static void WaitForTime(DateTime date)
        {
            while (date > DateTime.Now)
            {
                Thread.Sleep(100);
            }
        }

        public static void WaitTrue(Func<bool> confition, TimeSpan timeout)
        {
            var endTime = DateTime.Now + timeout;
            while (confition() == false)
            {
                if (DateTime.Now > endTime)
                {
                    throw new Exception("Ошибка таймаута WaitTrue");
                }
                Thread.Sleep(100);
            }
        }

        public static TestAccountInfo GetTestAccount()
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            var account = configDbServicesFactory.GetAccountService().GetOneOrNullBySystemName("TEST");
            if (account == null)
                throw new Exception("Тестовый аккаунт не существует");
            return TestAccountInfo.Create(account);
        }

        public static IDtoService GetDtoService(IApiService service)
        {
            var apiService = service as ApiService;
            if (apiService != null)
            {
                return apiService.DtoService;
            }
            var wrapper = service as ApiServiceWrapper;
            if (wrapper != null)
            {
                return GetDtoService(wrapper.ApiServiceInternal);
            }
            throw new Exception("Не удалось получить IDtoService");
        }

        public static IClient GetOfflineClient()
        {
            var config = new Config();
            var service = new FakeApiService();
            var client = new Client(service, config);
            return client;
        }

        public static void CheckExtentionProperties(
            Dictionary<string, object> properties1,
            ExtentionPropertyCollection properties2)
        {
            var prop = new ExtentionPropertyCollection(properties1);
            CheckExtentionProperties(prop, properties2);
        }

        public static void CheckExtentionProperties(ExtentionPropertyCollection properties1, ExtentionPropertyCollection properties2)
        {
            Assert.Equal(properties1.Count, properties2.Count);
            foreach (var property in properties1)
            {
                var property2 = properties2[property.Name];
                var value1 = property.Value.Value;
                var value2 = property2.Value;
                if (value1 == null)
                {
                    Assert.Null(value2);
                }
                else
                {
                    if (property.Value.DataType == Api.DataType.DateTime)
                    {
                        var date1 = (DateTime)value1;
                        var date2 = (DateTime)value2;
                        Assert.Equal(GetRoundDateTime(date1), GetRoundDateTime(date2));
                    }
                    else
                    {
                        Assert.Equal(property.Value.Value, property2.Value);
                    }
                }
                Assert.Equal(property.Value.DataType, property2.DataType);
            }
        }

        #region Check Component

        public static void CheckComponent(Api.ComponentInfo a, Api.ComponentInfo b)
        {
            Assert.Equal(a.DisplayName, b.DisplayName);
            Assert.Equal(a.SystemName, b.SystemName);
            CheckDateTimesEqualBySeconds(a.CreatedDate, b.CreatedDate);
            Assert.Equal(a.Id, b.Id);
            Assert.NotEqual(a.Id, Guid.Empty);
            Assert.Equal(a.ParentId, b.ParentId);
            Assert.Equal(a.Type.Id, b.Type.Id);
            Assert.Equal(a.Version, b.Version);
            CheckComponentType(a.Type, b.Type);
            CheckExtentionProperties(a.Properties, b.Properties);
        }

        public static void CheckComponent(
            IComponentControl parent,
            IComponentTypeControl typeControl,
            GetOrCreateComponentData createData,
            IComponentControl component)
        {
            Assert.False(component.IsFake());
            Assert.NotNull(component.Info);
            Assert.Equal(createData.DisplayName, component.Info.DisplayName);
            Assert.Equal(createData.SystemName, component.SystemName);
            Assert.Equal(createData.SystemName, component.Info.SystemName);
            Assert.True(component.Info.CreatedDate > DateTime.Now.AddDays(-1));
            Assert.True(component.Info.Id != Guid.Empty);
            Assert.NotNull(component.Info.ParentId);
            Assert.NotEqual(component.Info.ParentId, Guid.Empty);
            Assert.Equal(parent.Info.Id, component.Info.ParentId);
            Assert.Equal(typeControl.Info.Id, component.Info.Type.Id);
            Assert.Equal(typeControl.Info.Id, component.Type.Info.Id);
            Assert.Equal(createData.Version, component.Version);
            Assert.Equal(createData.Version, component.Info.Version);
            CheckExtentionProperties(createData.Properties, component.Info.Properties);
        }

        public static void CheckComponent(IComponentControl a, IComponentControl b)
        {
            Assert.False(a.IsFake());
            Assert.False(b.IsFake());
            CheckComponent(a.Info, b.Info);
            Assert.Equal(a.SystemName, b.SystemName);
            Assert.Equal(a.Version, b.Version);
        }

        public static void CheckComponent(GetOrCreateComponentData createData, IComponentControl control)
        {
            Assert.False(control.IsFake());
            CheckComponent(createData, control.Info);
            Assert.Equal(createData.SystemName, control.SystemName);
            Assert.Equal(createData.Version, control.Version);
        }

        public static void CheckComponent(GetOrCreateFolderData createData, IComponentControl control)
        {
            Assert.False(control.IsFake());
            CheckComponent(createData, control.Info);
            Assert.Equal(createData.SystemName, control.SystemName);
        }

        public static void CheckComponent(GetOrCreateComponentData createData, Api.ComponentInfo info)
        {
            Assert.Equal(createData.SystemName, info.SystemName);
            Assert.Equal(createData.DisplayName, info.DisplayName);
            Assert.Equal(createData.Version, info.Version);
            CheckExtentionProperties(createData.Properties, info.Properties);
        }

        public static void CheckComponent(GetOrCreateFolderData createData, Api.ComponentInfo info)
        {
            Assert.Equal(createData.SystemName, info.SystemName);
            Assert.Equal(createData.DisplayName, info.DisplayName);
            CheckExtentionProperties(createData.Properties, info.Properties);
        }

        public static void CheckChildComponent(
            Guid parentComponentId,
            Api.ComponentInfo component,
            List<Api.ComponentInfo> components)
        {
            var child = components.SingleOrDefault(x => x.Id == component.Id);
            Assert.NotNull(child);
            CheckComponent(component, child);
        }

        #endregion

        #region Check ComponentType

        public static void CheckComponentType(IComponentTypeControl a, IComponentTypeControl b)
        {
            Assert.False(a.IsFake());
            Assert.False(b.IsFake());
            Assert.Equal(a.SystemName, b.SystemName);
            CheckComponentType(a.Info, b.Info);
        }

        public static void CheckComponentType(Api.ComponentTypeInfo a, Api.ComponentTypeInfo b)
        {
            Assert.Equal(a.SystemName, b.SystemName);
            Assert.Equal(b.DisplayName, b.DisplayName);
            Assert.Equal(b.Id, b.Id);
            Assert.Equal(b.IsComponent(), b.IsComponent());
            Assert.Equal(b.IsFolder(), b.IsFolder());
            Assert.Equal(b.IsRoot(), b.IsRoot());
            Assert.Equal(b.IsSystem, b.IsSystem);
        }

        #endregion

        #region Check UnitTestType

        public static void CheckUnitTestType(GetOrCreateUnitTestTypeData createData, IUnitTestTypeControl control)
        {
            Assert.False(control.IsFake());
            Assert.Equal(createData.SystemName, control.SystemName);
            Assert.Equal(createData.SystemName, control.Info.SystemName);
            Assert.Equal(createData.DisplayName, control.Info.DisplayName);
        }

        public static void CheckUnitTestType(IUnitTestTypeControl a, IUnitTestTypeControl b)
        {
            Assert.False(a.IsFake());
            Assert.False(b.IsFake());
            Assert.Equal(a.Info.Id, b.Info.Id);
            Assert.Equal(a.SystemName, b.SystemName);
            Assert.Equal(a.SystemName, a.Info.SystemName);
            Assert.Equal(a.Info.SystemName, b.Info.SystemName);
            Assert.Equal(a.Info.DisplayName, b.Info.DisplayName);
            Assert.Equal(a.Info.IsSystem, b.Info.IsSystem);
        }

        #endregion

        public static void CheckEvent(SendEventBase eventBase, Api.EventInfo info)
        {
            Assert.Equal(eventBase.ComponentControl.Info.Id, info.OwnerId);
            //Assert.Equal(eventBase.Count, info.Count);
            Assert.Equal(eventBase.EventCategory.ToString(), info.Category.ToString());
            Assert.Equal(eventBase.Importance, info.Importance);
            Assert.Equal(eventBase.TypeSystemName, info.TypeSystemName);
            if (eventBase.TypeDisplayName == null)
            {
                Assert.Equal(info.TypeSystemName, info.TypeDisplayName);
            }
            else
            {
                Assert.Equal(eventBase.TypeDisplayName, info.TypeDisplayName);
            }
            Assert.Equal(eventBase.Version, info.Version);
            //CheckDateTimesEqualBySeconds(eventBase.StartDate, info.StartDate);
            //CheckDateTimesEqualBySeconds(eventBase.EndDate, info.EndDate);
            Assert.Equal(eventBase.JoinKey, info.JoinKeyHash);//todo переименовать, чтобы ыбло одинаково
            Assert.Equal(eventBase.Message, info.Message);
            CheckExtentionProperties(eventBase.Properties, info.Properties);
        }

        public static void InitRandomProperties(ExtentionPropertyCollection properties)
        {
            properties.Set("bool", RandomHelper.GetRandomBool());
            properties.Set("int32", RandomHelper.GetRandomInt32(int.MinValue, int.MaxValue));
            properties.Set("int64", RandomHelper.GetRandomInt64(long.MinValue, long.MaxValue));
            var d = RandomHelper.GetRandomInt32(-1000000, 1000000) + 0.54237;
            properties.Set("double", d);
            properties.Set("datetime", DateTime.Now);
            properties.Set("string", "random string " + PasswordHelper.GetRandomPassword(300));
            properties.Set("binary", RandomHelper.GetRandomBytes(300));
            properties.Set("guid", Guid.NewGuid());

            // продублируем
            properties.Set("bool_2", RandomHelper.GetRandomBool());
            properties.Set("int32_2", RandomHelper.GetRandomInt32(int.MinValue, int.MaxValue));
            properties.Set("int64_2", RandomHelper.GetRandomInt64(long.MinValue, long.MaxValue));
            d = RandomHelper.GetRandomInt32(-1000000, 1000000) + 0.54237;
            properties.Set("double_2", d);
            properties.Set("datetime_2", DateTime.Now.AddMinutes(-5));
            properties.Set("string_2", "random string " + PasswordHelper.GetRandomPassword(300));
            properties.Set("binary_2", RandomHelper.GetRandomBytes(300));
            properties.Set("guid_2", Guid.NewGuid());
        }

        public static void InitRandomProperties(List<Api.Dto.ExtentionPropertyDto> propertyDtos)
        {
            var properties = new ExtentionPropertyCollection();
            InitRandomProperties(properties);
            var propertiesDto2 = DataConverter.GetExtentionPropertyDtos(properties);
            propertyDtos.AddRange(propertiesDto2);
        }

        public static IComponentTypeControl GetRandomComponentTypeControl(IClient client)
        {
            var data = GetRandomGetOrCreateComponentTypeData();
            return client.GetOrCreateComponentTypeControl(data);
        }

        public static GetOrCreateComponentTypeData GetRandomGetOrCreateComponentTypeData()
        {
            var guid = Guid.NewGuid().ToString();
            var data = new GetOrCreateComponentTypeData("systemName " + guid)
            {
                DisplayName = "display name " + guid
            };
            return data;
        }

        public static IComponentControl CreateRandomComponent()
        {
            var account = GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();
            var message = GetRandomGetOrCreateComponentData(client);
            return root.GetOrCreateChildComponentControl(message);
        }

        public static string GetRandomVersion()
        {
            return
                "1"
                + "." + RandomHelper.GetRandomByte()
                + "." + RandomHelper.GetRandomByte()
                + "." + RandomHelper.GetRandomByte();
        }

        public static GetOrCreateComponentData GetRandomGetOrCreateComponentData(IClient client)
        {
            var type = GetRandomComponentTypeControl(client);
            var systemName = "TestWebSite " + Guid.NewGuid();
            var data = new GetOrCreateComponentData(systemName, type)
            {
                Version = GetRandomVersion(),
                DisplayName = systemName + " display name"
            };
            InitRandomProperties(data.Properties);
            return data;
        }

        public static GetOrCreateFolderData GetRandomGetOrCreateFolderData()
        {
            var systemName = "TestWebSite " + Guid.NewGuid();
            var data = new GetOrCreateFolderData(systemName)
            {
                DisplayName = systemName + " display name"
            };
            InitRandomProperties(data.Properties);
            return data;
        }

        public static DateTime GetServerDateTime()
        {
            var account = GetTestAccount();
            var client = account.GetClient();
            return GetRoundDateTime(client.ToServerTime(DateTime.Now));
        }

        public static void AreEqual(byte[] x, byte[] y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            if (x.Length != y.Length)
            {
                throw new Exception("x.Length != y.Length");
            }
            for (var i = 0; i < x.Length; i++)
            {
                Assert.Equal(x[i], y[i]);
            }
        }

        public static bool CheckDateTimesEqualBySeconds(DateTime? date1, DateTime? date2)
        {
            if (!date1.HasValue && !date2.HasValue)
                return true;

            if (!date1.HasValue || !date2.HasValue)
                return false;

            return GetRoundDateTime(date1.Value) == GetRoundDateTime(date2.Value);
        }

        public static bool UseLocalDispatcher()
        {
            return ConfigurationManager.AppSettings["UseLocalDispatcher"] == "true";
        }

        public IDispatcherService GetDispatcherService()
        {
            return DispatcherHelper.GetDispatcherService();
        }

        public static IApiService GetWebService(Api.AccessToken accessToken, string accountName)
        {
            if (UseLocalDispatcher())
            {
                var dispatcher = DispatcherService.Wrapper;
                var dtoService = new ApiToDispatcherAdapter(dispatcher, "2.2.2.2", accountName);
                var apiService = new ApiService(dtoService, accessToken);
                return apiService;
            }
            var uri = ApiHelper.GetApiUrl(accountName);
            if (CanRunApiOnIisExpress && uri.AbsoluteUri.StartsWith("http://localhost"))
            {
                CanRunApiOnIisExpress = false;
                IisExpress.StartApiWebService();
            }
            var dtoService2 = new DtoServiceProxy(uri);
            return new ApiService(dtoService2, accessToken);
        }

        public static Api.LogLevel GetRandomLogLevel()
        {
            var levels = new List<Api.LogLevel>()
            {
                Api.LogLevel.Debug,
                Api.LogLevel.Info,
                Api.LogLevel.Trace,
                Api.LogLevel.Warning,
                Api.LogLevel.Error,
                Api.LogLevel.Fatal
            };
            return RandomHelper.GetRandomItemFromList(levels);
        }

        public static Api.EventImportance GetRandomEventImportance()
        {
            var levels = new List<Api.EventImportance>()
            {
                Api.EventImportance.Success,
                Api.EventImportance.Warning,
                Api.EventImportance.Alarm
            };
            return RandomHelper.GetRandomItemFromList(levels);
        }

        public static string GetRandomSystemName()
        {
            return "Random System Name " + Guid.NewGuid();
        }

        public static string GetRandomDisplayName()
        {
            return "Random Display Name " + Guid.NewGuid();
        }

        public static GetOrCreateUnitTestTypeData GetRandomGetOrCreateUnitTestTypeData()
        {
            var systemName = GetRandomSystemName();
            return new GetOrCreateUnitTestTypeData(systemName)
            {
                DisplayName = GetRandomDisplayName()
            };
        }

        public static ApplicationErrorData CreateRandomApplicationError(IComponentControl component)
        {
            var randomName = GetRandomSystemName();
            var eventData = component.CreateApplicationError("RandomApplicationError " + randomName);
            InitRandomEvent(eventData);
            return eventData;
        }

        public static ComponentEventData CreateRandomComponentEvent(IComponentControl component)
        {
            var randomName = GetRandomSystemName();
            var eventData = component.CreateComponentEvent("RandomComponentEvent " + randomName);
            InitRandomEvent(eventData);
            return eventData;
        }

        public static void InitRandomEvent(SendEventBase eventBase)
        {
            eventBase.Message = "test comment " + PasswordHelper.GetRandomPassword(3000);
            eventBase.StartDate = DateTime.Now;
            eventBase.Importance = GetRandomEventImportance();
            eventBase.JoinInterval = TimeSpan.FromSeconds(RandomHelper.GetRandomInt32(0, 1000000));
            eventBase.JoinKey = RandomHelper.GetRandomInt64(0, 100000000000);
            InitRandomProperties(eventBase.Properties);
        }

        public static void CheckFolderType(Api.ComponentTypeInfo componentTypeInfo)
        {
            Assert.Equal(componentTypeInfo.Id, Core.AccountsDb.SystemComponentType.Folder.Id);
            Assert.Equal(componentTypeInfo.SystemName, Core.AccountsDb.SystemComponentType.Folder.SystemName);
        }

        public static IMetricTypeCacheReadObject CreateTestMetricType(Guid accountId)
        {
            var name = "MetricType." + Guid.NewGuid();

            var service = new MetricService(GetStorage(accountId));
            var type = service.GetOrCreateType(accountId, name);
            return type;
        }

        public static IMetricCacheReadObject CreateTestMetric(Guid accountId, Guid componentId)
        {
            var type = CreateTestMetricType(accountId);

            var service = new MetricService(GetStorage(accountId));
            var metricId = service.CreateMetric(accountId, componentId, type.Id);
            return AllCaches.Metrics.Find(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = metricId
            });
        }

        public static ComponentTypeForRead CreateRandomComponentType(Guid accountId)
        {
            var guid = Guid.NewGuid().ToString();

            var data = new GetOrCreateComponentTypeRequestData()
            {
                DisplayName = "display name " + guid,
                SystemName = "sysName " + guid
            };
            var service = new ComponentTypeService(GetStorage(accountId));
            var type = service.GetOrCreateComponentType(accountId, data);
            return type;
        }

        public static Core.Api.ComponentInfo GetTestApplicationComponent(TestAccountInfo accountInfo)
        {
            var componentTypeId = CreateRandomComponentTypeId(accountInfo.Id);
            var data = new GetOrCreateComponentRequestData()
            {
                SystemName = "test component systemName",
                DisplayName = "test component dispay name",
                TypeId = componentTypeId,
                ParentComponentId = accountInfo.RootId
            };
            var dispatcher = GetDispatcherClient();
            var response = dispatcher.GetOrCreateComponent(accountInfo.Id, data);
            return response.Data.Component;
        }

        public static DateTime GetRoundDateTime(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }

        public static DateTime GetNow()
        {
            return GetRoundDateTime(DateTime.Now);
        }

        public static void AreEqual(DateTime a, DateTime b, TimeSpan maxGap)
        {
            var duration = a - b;
            var durationAbs = Math.Abs(duration.TotalSeconds);
            if (durationAbs > maxGap.TotalSeconds)
            {
                string error = string.Format("Разница между {0} и {1} больше чем {2}", a, b, maxGap);
                throw new Exception(error);
            }
        }

        public static DispatcherClient GetDispatcherClient()
        {
            return new DispatcherClient("Tests");
        }

        public static DatabaseInfo GetTestAccountDatabase(Guid accountId)
        {
            var client = GetDispatcherClient();
            var account = client.GetAccountById(new GetAccountByIdRequestData() { Id = accountId }).Data;
            var database = client.GetDatabaseById(new GetDatabaseByIdRequestData() { Id = account.AccountDatabaseId }).Data;
            return database;
        }

        public static EventTypeForRead GetOrCreateEventType(
            Guid accountId,
            Storage.EventCategory category,
            string systemName)
        {
            var eventType = new EventTypeForAdd()
            {
                SystemName = systemName,
                DisplayName = "display name for " + systemName,
                Category = category
            };
            var type = new EventTypeService(GetStorage(accountId)).GetOrCreate(eventType, accountId);
            return type;
        }

        public static UserForRead GetAccountAdminUser(Guid accountId)
        {
            var service = new UserService(GetStorage(accountId));
            return service.GetAccountAdmin();
        }

        public static UserForRead CreateTestUser(Guid accountId, string password = null, string displayName = null)
        {
            var storage = GetStorage(accountId);
            var userService = new UserService(storage);
            var login = "Test.User." + Guid.NewGuid();
            var user = new UserForAdd()
            {
                Id = Guid.NewGuid(),
                CreateDate = GetNow(),
                Login = login,
                DisplayName = displayName,
                LastName = login,
                FirstName = login,
                MiddleName = login,
                Post = "Post"
            };

            if (!string.IsNullOrEmpty(password))
                user.PasswordHash = PasswordHelper.GetPasswordHashString(password);

            // Создадим по одному контакту каждого типа
            var contacts = new List<UserContactForAdd>();
            contacts.Add(new UserContactForAdd()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = UserContactType.MobilePhone,
                Value = Guid.NewGuid().ToString(),
                CreateDate = GetNow()
            });

            contacts.Add(new UserContactForAdd()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = UserContactType.Telegram,
                Value = Guid.NewGuid().ToString(),
                CreateDate = GetNow()
            });

            contacts.Add(new UserContactForAdd()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = UserContactType.VKontakte,
                Value = Guid.NewGuid().ToString(),
                CreateDate = GetNow()
            });

            var userId = userService.CreateUser(user, contacts, new List<UserRoleForAdd>(), accountId, false);

            return storage.Users.GetOneById(userId);
        }

        public static Guid CreateRandomComponentTypeId(Guid accountId)
        {
            return CreateRandomComponentType(accountId).Id;
        }

        public static EventTypeForRead GetTestEventType(Guid accountId)
        {
            var eventType = new EventTypeForAdd()
            {
                Category = Storage.EventCategory.ComponentEvent,
                DisplayName = "Тестовый тип события " + Guid.NewGuid(),
                ImportanceForNew = Storage.EventImportance.Success,
                JoinIntervalSeconds = 5,
                SystemName = "EventType.Test " + Guid.NewGuid()
            };
            return new EventTypeService(GetStorage(accountId)).GetOrCreate(eventType, accountId);
        }

        public static IUnitTestTypeCacheReadObject CreateTestUnitTestType(Guid accountId)
        {
            var service = new UnitTestTypeService(GetStorage(accountId));
            var unitTestType = service.GetOrCreateUnitTestType(accountId, new GetOrCreateUnitTestTypeRequestData()
            {
                DisplayName = "Тестовый тип проверки " + Guid.NewGuid(),
                SystemName = "UnitTestType.Test " + Guid.NewGuid()
            });
            return unitTestType;
        }

        public static GetOrCreateUnitTestResponseData CreateTestUnitTest(Guid accountId, Guid componentId)
        {
            var unitTestType = CreateTestUnitTestType(accountId);
            var client = GetDispatcherClient();
            var response = client.GetOrCreateUnitTest(accountId, new GetOrCreateUnitTestRequestData()
            {
                ComponentId = componentId,
                DisplayName = "Тестовая проверка " + DateTime.Now.Ticks,
                SystemName = "UnitTest.Test " + DateTime.Now.Ticks,
                UnitTestTypeId = unitTestType.Id
            });
            return response.Data;
        }

        public static void CheckActualStatusEventsCount(TestAccountInfo account, Guid ownerId)
        {
            account.SaveAllCaches();

            var now = GetDispatcherClient().GetServerTime().Data.Date.AddSeconds(1);
            using (var storageDbContext = GetAccountDbContext(account.Id))
            {
                var events = storageDbContext.Events
                    .Where(x => x.OwnerId == ownerId && x.ActualDate > now)
                    .ToList();

                var categoryGroups = events.GroupBy(x => x.Category);
                foreach (var categoryGroup in categoryGroups)
                {
                    var categoryEvents = categoryGroup.ToArray();
                    Assert.Equal(1, categoryEvents.Length);
                }
            }
        }

        public static DbMetricType CreateCpuMetricType(Guid accountId)
        {
            using (var accountContext = GetAccountDbContext(accountId))
            {
                var metricType = new DbMetricType()
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                    SystemName = "CPU" + Guid.NewGuid(),
                    ConditionAlarm = "value == 100",
                    ConditionWarning = "value > 90",
                    ConditionElseColor = ObjectColor.Green,
                    NoSignalColor = ObjectColor.Red
                };
                accountContext.MetricTypes.Add(metricType);
                accountContext.SaveChanges();
                return metricType;
            }
        }

        public static DbMetricType CreateHddMetricType(Guid accountId)
        {
            using (var accountContext = GetAccountDbContext(accountId))
            {
                var metricType = new DbMetricType()
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                    SystemName = "HDD." + Guid.NewGuid(),
                    ConditionAlarm = "value < 1",
                    ConditionWarning = "value < 10",
                    ConditionElseColor = ObjectColor.Green,
                    NoSignalColor = ObjectColor.Red
                };
                accountContext.MetricTypes.Add(metricType);
                accountContext.SaveChanges();
                return metricType;
            }
        }

        public static void CheckExternalStatus(Guid accountId, IComponentControl componentControl, Storage.MonitoringStatus status)
        {
            using (var accountDbContext = GetAccountDbContext(accountId))
            {
                var component = accountDbContext.Components.First(x => x.Id == componentControl.Info.Id);
                Assert.Equal(component.ExternalStatus.Status, status);
            }
        }

        public static void CheckMetricaStatus(Guid accountId, IComponentControl componentControl, string metricName, Storage.MonitoringStatus status)
        {
            using (var accountDbContext = GetAccountDbContext(accountId))
            {
                var component = accountDbContext.Components.First(x => x.Id == componentControl.Info.Id);
                var metric = component.Metrics.Single(x => x.MetricType.SystemName == metricName && x.IsDeleted == false);
                Assert.Equal(metric.Bulb.Status, status);
            }
        }

        public static List<DbNotification> SendSubscriptionNotifications(
            Guid accountId,
            Storage.EventImportance importance,
            Guid componentId,
            SubscriptionChannel notificationType)
        {
            using (var accountDbContext = GetAccountDbContext(accountId))
            {
                // получим новые, неотправленные уведомления
                var notifications = accountDbContext.Notifications
                    .Where(x => x.Event.OwnerId == componentId &&
                                x.Status == NotificationStatus.InQueue &&
                                x.Event.Importance == importance &&
                                x.Type == notificationType)
                    .ToList();

                foreach (var notification in notifications)
                {
                    // сделаем вид, что уведомления отправлены
                    notification.SendDate = DateTime.Now;
                    notification.Status = NotificationStatus.Processed;
                }

                accountDbContext.SaveChanges();

                return notifications;
            }
        }

        public static GetOrCreateComponentTypeData GetRamdomGetOrCreateComponentTypeData()
        {
            string guid = Guid.NewGuid().ToString();
            var data = new GetOrCreateComponentTypeData("systemName " + guid)
            {
                DisplayName = "display name " + guid
            };
            return data;
        }

        public static IStorage GetStorage(Guid accountId)
        {
            return AccountStorageFactory.GetStorageByAccountId(accountId);
        }

        private static readonly IAccountStorageFactory AccountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();

        internal static AccountDbContext GetAccountDbContext(Guid accountId)
        {
            var dispatcherClient = DispatcherHelper.GetDispatcherClient();
            var accountInfo = dispatcherClient.GetAccountById(new GetAccountByIdRequestData() { Id = accountId }).Data;
            var databaseInfo = dispatcherClient.GetDatabaseById(new GetDatabaseByIdRequestData() { Id = accountInfo.AccountDatabaseId }).Data;
            return AccountDbContext.CreateFromConnectionString(null, databaseInfo.ConnectionString);
        }

    }
}
