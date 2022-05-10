using System;
using Xunit;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Caching
{
    public class OtherCacheTests : BaseTest
    {
        [Fact]
        public void SendUnitTestAfterEnablingComponentTest()
        {
            // Создадим отключенный компонент и проверку до создания диспетчера
            Guid unitTestId;
            var account = TestHelper.GetTestAccount();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();
            var componentService = new ComponentService(TestHelper.GetStorage(), timeService);
            var component = componentService.GetOrCreateComponent(new GetOrCreateComponentRequestDataDto()
            {
                DisplayName = Ulid.NewUlid().ToString(),
                SystemName = Ulid.NewUlid().ToString(),
                TypeId = SystemComponentType.Others.Id
            });
            componentService.DisableComponent(component.Id, null, null);

            var unitTestTypeService = new UnitTestTypeService(TestHelper.GetStorage(), timeService);
            var unitTestType = unitTestTypeService.GetOrCreateUnitTestType(new GetOrCreateUnitTestTypeRequestDataDto()
            {
                SystemName = "Main",
                DisplayName = "Main"
            });

            var unitTestService = new UnitTestService(TestHelper.GetStorage(), timeService);
            var unitTestCache = unitTestService.GetOrCreateUnitTest(new GetOrCreateUnitTestRequestDataDto()
            {
                ComponentId = component.Id,
                SystemName = Ulid.NewUlid().ToString(),
                DisplayName = Ulid.NewUlid().ToString(),
                UnitTestTypeId = unitTestType.Id
            });

            unitTestId = unitTestCache.Id;

            unitTestService.SendUnitTestResult(new SendUnitTestResultRequestDataDto()
            {
                UnitTestId = unitTestCache.Id,
                Result = UnitTestResult.Unknown,
                ActualIntervalSeconds = 60
            });
            // Создадим диспетчер
            var dispatcher = TestHelper.GetDispatcherClient();

            // Включим компонент
            var componentEnableResponse = dispatcher.SetComponentEnable(component.Id);
            componentEnableResponse.Check();

            // Проверим, что компонент стал серым
            var getComponentTotalStateResponse = dispatcher.GetComponentTotalState(component.Id, true);
            var componentState = getComponentTotalStateResponse.GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Unknown, componentState.Status);

            // Отправим проверку
            var sendUnitTestResultResponse = dispatcher.SendUnitTestResult(new SendUnitTestResultRequestDataDto()
            {
                UnitTestId = unitTestId,
                Result = UnitTestResult.Success,
                ActualIntervalSeconds = 60,
                Message = "OK"
            });
            sendUnitTestResultResponse.Check();

            // Проверим, что она отправилась
            var getUnitTestStateResponse = dispatcher.GetUnitTestState(unitTestId);
            var unitTestState = getUnitTestStateResponse.GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Success, unitTestState.Status);

            // Проверим, что компонент стал зелёным
            getComponentTotalStateResponse = dispatcher.GetComponentTotalState(component.Id, true);
            componentState = getComponentTotalStateResponse.GetDataAndCheck();
            Assert.Equal(MonitoringStatus.Success, componentState.Status);
        }

        [Fact]
        public void AccountsCacheTest()
        {
            var dispatcher = TestHelper.GetDispatcherClient();
            var r1 = dispatcher.GetServerTime();
            var r2 = dispatcher.GetServerTime();
        }
    }
}
