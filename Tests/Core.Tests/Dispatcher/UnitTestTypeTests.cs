using System;
using System.Threading;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class UnitTestTypeTests
    {
        [Fact]
        public void GetOrCreateUnitTestTypeTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();

            var data = new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "UnitTestType." + Guid.NewGuid()
            };

            var response = dispatcher.GetOrCreateUnitTestType(account.Id, data);
            response.Check();

            var unitTestTypeId = response.Data.Id;

            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestTypeRepository = context.GetUnitTestTypeRepository();
                var unitTestType = unitTestTypeRepository.GetByIdOrNull(unitTestTypeId);
                Assert.NotNull(unitTestType);
                Assert.Equal(data.SystemName, unitTestType.SystemName);
            }

        }

        [Fact]
        public void ChangeNoSignalColorTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Создадим новый тип проверки и новую проверку
            var unitTestType = dispatcher.GetOrCreateUnitTestType(account.Id, new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "UnitTestType." + Guid.NewGuid()
            }).Data;

            var unitTest = dispatcher.GetOrCreateUnitTest(account.Id, new GetOrCreateUnitTestRequestData()
            {
                UnitTestTypeId = unitTestType.Id,
                ComponentId = component.Id,
                SystemName = "UnitTest." + Guid.NewGuid()
            }).Data;

            // Отправим зелёную проверку, актуальную 1 секунду
            dispatcher.SendUnitTestResult(account.Id, new SendUnitTestResultRequestData()
            {
                UnitTestId = unitTest.Id,
                Result = UnitTestResult.Success,
                ActualIntervalSeconds = 1
            }).Check();

            // Подождём 2 секунды
            Thread.Sleep(2000);

            // Получим цвет проверки
            var response = dispatcher.GetUnitTestState(account.Id, component.Id, unitTest.Id);
            response.Check();

            // Проверим, что проверка стала красной
            Assert.Equal(MonitoringStatus.Alarm, response.Data.Status);

            // Поменяем "Цвет если нет сигнала" на жёлтый
            dispatcher.UpdateUnitTestType(account.Id, new UpdateUnitTestTypeRequestData()
            {
                UnitTestTypeId = unitTestType.Id,
                SystemName = unitTestType.SystemName,
                DisplayName = unitTestType.DisplayName,
                ActualTimeSecs = unitTestType.ActualTimeSecs,
                NoSignalColor = ObjectColor.Yellow
            }).Check();

            // Получим цвет проверки
            response = dispatcher.GetUnitTestState(account.Id, component.Id, unitTest.Id);
            response.Check();

            // Проверим, что проверка стала жёлтой
            Assert.Equal(MonitoringStatus.Warning, response.Data.Status);
            Assert.False(response.Data.HasSignal);
        }
    }
}
