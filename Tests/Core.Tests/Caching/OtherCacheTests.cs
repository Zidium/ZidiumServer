using System;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Caching
{
    public class OtherCacheTests
    {
        [Fact]
        public void SendUnitTestAfterEnablingComponentTest()
        {
            // Создадим отключенный компонент и проверку до создания диспетчера
            Guid unitTestId;
            var account = TestHelper.GetTestAccount();
            var componentService = new ComponentService(TestHelper.GetStorage(account.Id));
            var component = componentService.GetOrCreateComponent(account.Id, new GetOrCreateComponentRequestData()
            {
                DisplayName = Guid.NewGuid().ToString(),
                SystemName = Guid.NewGuid().ToString(),
                TypeId = SystemComponentType.Others.Id
            });
            componentService.DisableComponent(account.Id, component.Id, null, null);

            var unitTestTypeService = new UnitTestTypeService(TestHelper.GetStorage(account.Id));
            var unitTestType = unitTestTypeService.GetOrCreateUnitTestType(account.Id, new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "Main",
                DisplayName = "Main"
            });

            var unitTestService = new UnitTestService(TestHelper.GetStorage(account.Id));
            var unitTestCache = unitTestService.GetOrCreateUnitTest(account.Id, new GetOrCreateUnitTestRequestData()
            {
                ComponentId = component.Id,
                SystemName = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                UnitTestTypeId = unitTestType.Id
            });

            unitTestId = unitTestCache.Id;

            unitTestService.SendUnitTestResult(account.Id, new SendUnitTestResultRequestData()
            {
                UnitTestId = unitTestCache.Id,
                Result = UnitTestResult.Unknown,
                ActualIntervalSeconds = 60
            });
            // Создадим диспетчер
            var dispatcher = TestHelper.GetDispatcherClient();

            // Включим компонент
            var componentEnableResponse = dispatcher.SetComponentEnable(account.Id, component.Id);
            componentEnableResponse.Check();

            // Проверим, что компонент стал серым
            var getComponentTotalStateResponse = dispatcher.GetComponentTotalState(account.Id, component.Id, true);
            var componentState = getComponentTotalStateResponse.Data;
            Assert.Equal(MonitoringStatus.Unknown, componentState.Status);

            // Отправим проверку
            var sendUnitTestResultResponse = dispatcher.SendUnitTestResult(account.Id, new SendUnitTestResultRequestData()
            {
                UnitTestId = unitTestId,
                Result = UnitTestResult.Success,
                ActualIntervalSeconds = 60,
                Message = "OK"
            });
            sendUnitTestResultResponse.Check();

            // Проверим, что она отправилась
            var getUnitTestStateResponse = dispatcher.GetUnitTestState(account.Id, component.Id, unitTestId);
            var unitTestState = getUnitTestStateResponse.Data;
            Assert.Equal(MonitoringStatus.Success, unitTestState.Status);

            // Проверим, что компонент стал зелёным
            getComponentTotalStateResponse = dispatcher.GetComponentTotalState(account.Id, component.Id, true);
            componentState = getComponentTotalStateResponse.Data;
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
