using System;
using System.Threading;
using Xunit;
using Zidium.Core.Api;
using Zidium.Api.Dto;
using Zidium.TestTools;
using Zidium.Common;
using System.Collections.Generic;

namespace Zidium.Core.Tests.Dispatcher
{
    public class UnittestTests : BaseTest
    {
        [Fact]
        public void CreateUnittestWithoutTypeTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();
            var component = account.CreateTestApplicationComponent();

            var data = new GetOrCreateUnitTestRequestDataDto()
            {
                ComponentId = component.Id,
                UnitTestTypeId = null,
                SystemName = "Unittest." + Ulid.NewUlid()
            };

            var unittest = dispatcher.GetOrCreateUnitTest(data).GetDataAndCheck();
            var unittestType = dispatcher.GetUnitTestTypeById(unittest.TypeId).GetDataAndCheck();
            Assert.Equal("CustomUnitTestType", unittestType.SystemName);
        }

        [Fact]
        public void SendUnitTestResultsTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();
            var component = account.CreateTestApplicationComponent();
            var unittest1 = TestHelper.CreateTestUnitTest(component.Id);
            var unittest2 = TestHelper.CreateTestUnitTest(component.Id);

            var data = new List<SendUnitTestResultRequestDataDto>
            {
                new SendUnitTestResultRequestDataDto()
                {
                    UnitTestId = unittest1.Id,
                    Result = UnitTestResult.Success
                },
                new SendUnitTestResultRequestDataDto()
                {
                    UnitTestId = unittest2.Id,
                    Result = UnitTestResult.Warning
                }
            };

            dispatcher.SendUnitTestResults(data);

            var status1 = dispatcher.GetUnitTestState(unittest1.Id).GetDataAndCheck().Status;
            var status2 = dispatcher.GetUnitTestState(unittest2.Id).GetDataAndCheck().Status;

            Assert.Equal(MonitoringStatus.Success, status1);
            Assert.Equal(MonitoringStatus.Warning, status2);
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
                SystemName = "UnitTestType." + Ulid.NewUlid()
            }).GetDataAndCheck();

            var unitTest = dispatcher.GetOrCreateUnitTest(new GetOrCreateUnitTestRequestDataDto()
            {
                UnitTestTypeId = unitTestType.Id,
                ComponentId = component.Id,
                SystemName = "UnitTest." + Ulid.NewUlid()
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
            dispatcher.UpdateUnitTest(new UpdateUnitTestRequestData()
            {
                UnitTestId = unitTest.Id,
                DisplayName = unitTest.DisplayName,
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
