using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Zidium.Api;
using Zidium.Api.Common;
using Zidium.Api.Dto;
using Zidium.Api.XmlConfig;
using Xunit;
using Zidium.Core.Common;

namespace Zidium.TestTools
{
    /// <summary>
    /// Хелпер для юнит-тестов
    /// </summary>
    public class TestHelper
    {
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
                var appDir = Tools.GetApplicationDir();

                var builder = new ConfigurationBuilder()
                    .SetBasePath(appDir)
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile("appsettings.user.json", true);

                var configuration = builder.Build();

                var secretKey = configuration["SecretKey"];
                return new TestAccountInfo()
                {
                    SystemName = configuration["AccountName"],
                    SecretKey = secretKey,
                    Token = new AccessToken()
                    {
                        SecretKey = secretKey
                    }
                };
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
                    if (property.Value.DataType == DataType.DateTime)
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

        public static void CheckComponent(ComponentInfo a, ComponentInfo b)
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

        public static void CheckComponent(GetOrCreateComponentData createData, ComponentInfo info)
        {
            Assert.Equal(createData.SystemName, info.SystemName);
            Assert.Equal(createData.DisplayName, info.DisplayName);
            Assert.Equal(createData.Version, info.Version);
            CheckExtentionProperties(createData.Properties, info.Properties);
        }

        public static void CheckComponent(GetOrCreateFolderData createData, ComponentInfo info)
        {
            Assert.Equal(createData.SystemName, info.SystemName);
            Assert.Equal(createData.DisplayName, info.DisplayName);
            CheckExtentionProperties(createData.Properties, info.Properties);
        }


        public static void CheckChildComponent(
            Guid parentComponentId,
            ComponentInfo component,
            List<ComponentInfo> components)
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

        public static void CheckComponentType(ComponentTypeInfo a, ComponentTypeInfo b)
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

        public static void CheckEvent(SendEventBase eventBase, EventInfo info)
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

        public static void InitRandomProperties(List<ExtentionPropertyDto> propertyDtos)
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

        /// <summary>
        /// Убираем миллисекунды
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNow()
        {
            return GetRoundDateTime(DateTime.Now);
        }

        public static DateTime GetServerDateTime()
        {
            var account = GetTestAccount();
            var client = account.GetClient();
            return GetRoundDateTime(client.ToServerTime(DateTime.Now));
        }

        public static void AreEqual(DateTime a, DateTime b, TimeSpan maxGap)
        {
            var duration = a - b;
            var durationAbs = Math.Abs(duration.TotalSeconds);
            if (durationAbs > maxGap.TotalSeconds)
            {
                var error = string.Format("Разница между {0} и {1} больше чем {2}", a, b, maxGap);
                throw new Exception(error);
            }
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

        /// <summary>
        /// Округляет дату до секунд
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetRoundDateTime(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }

        public static bool CheckDateTimesEqualBySeconds(DateTime? date1, DateTime? date2)
        {
            if (!date1.HasValue && !date2.HasValue)
                return true;

            if (!date1.HasValue || !date2.HasValue)
                return false;

            return GetRoundDateTime(date1.Value) == GetRoundDateTime(date2.Value);
        }

        public static LogLevel GetRandomLogLevel()
        {
            var levels = new List<LogLevel>()
            {
                LogLevel.Debug,
                LogLevel.Info,
                LogLevel.Trace,
                LogLevel.Warning,
                LogLevel.Error,
                LogLevel.Fatal
            };
            return RandomHelper.GetRandomItemFromList(levels);
        }

        public static EventImportance GetRandomEventImportance()
        {
            var levels = new List<EventImportance>()
            {
                EventImportance.Success,
                EventImportance.Warning,
                EventImportance.Alarm
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

        public static void CheckFolderType(ComponentTypeInfo componentTypeInfo)
        {
            Assert.Equal(componentTypeInfo.Id, SystemComponentType.Folder.Id);
            Assert.Equal(componentTypeInfo.SystemName, SystemComponentType.Folder.SystemName);
        }

    }
}
