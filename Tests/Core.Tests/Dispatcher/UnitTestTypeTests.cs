using System;
using System.Threading;
using Xunit;
using Zidium.Core.Api;
using Zidium.Api.Dto;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class UnitTestTypeTests : BaseTest
    {
        [Fact]
        public void GetOrCreateUnitTestTypeTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();

            var data = new GetOrCreateUnitTestTypeRequestDataDto()
            {
                SystemName = "UnitTestType." + Guid.NewGuid()
            };

            var response = dispatcher.GetOrCreateUnitTestType(data);
            response.Check();

            var unitTestTypeId = response.GetDataAndCheck().Id;

            using (var context = TestHelper.GetDbContext())
            {
                var unitTestType = context.UnitTestTypes.Find(unitTestTypeId);
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
            var unitTestType = dispatcher.GetOrCreateUnitTestType(new GetOrCreateUnitTestTypeRequestDataDto()
            {
                SystemName = "UnitTestType." + Guid.NewGuid()
            }).GetDataAndCheck();

            var unitTest = dispatcher.GetOrCreateUnitTest(new GetOrCreateUnitTestRequestDataDto()
            {
                UnitTestTypeId = unitTestType.Id,
                ComponentId = component.Id,
                SystemName = "UnitTest." + Guid.NewGuid()
            }).GetDataAndCheck();

            // Отправим зелёную проверку, актуальную 1 секунду
            dispatcher.SendUnitTestResult(new SendUnitTestResultRequestDataDto()
            {
                UnitTestId = unitTest.Id,
                Result = UnitTestResult.Success,
                ActualIntervalSeconds = 1
            }).Check();

            // Подождём 2 секунды
            Thread.Sleep(2000);

            // Получим цвет проверки
            var response = dispatcher.GetUnitTestState(unitTest.Id);
            response.Check();

            // Проверим, что проверка стала красной
            Assert.Equal(MonitoringStatus.Alarm, response.GetDataAndCheck().Status);

            // Поменяем "Цвет если нет сигнала" на жёлтый
            dispatcher.UpdateUnitTestType(new UpdateUnitTestTypeRequestData()
            {
                UnitTestTypeId = unitTestType.Id,
                SystemName = unitTestType.SystemName,
                DisplayName = unitTestType.DisplayName,
                ActualTimeSecs = unitTestType.ActualTimeSecs,
                NoSignalColor = ObjectColor.Yellow
            }).Check();

            // Получим цвет проверки
            response = dispatcher.GetUnitTestState(unitTest.Id);
            response.Check();

            // Проверим, что проверка стала жёлтой
            Assert.Equal(MonitoringStatus.Warning, response.GetDataAndCheck().Status);
            Assert.False(response.GetDataAndCheck().HasSignal);
        }
    }
}
