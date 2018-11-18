using System;
using System.Linq;
using System.Threading;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class ChecksTests
    {
        [Fact]
        public void AddPingCheckTest()
        {
            // Добавим проверку ping
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            var data = new AddPingUnitTestRequestData()
            {
                ComponentId = component.Id,
                SystemName = "Ping_" + Guid.NewGuid(),
                PeriodSeconds = 600,
                ErrorColor = UnitTestResult.Alarm,
                Host = Guid.NewGuid() + ".ru",
                TimeoutMs = 1000,
                AttempMax = 3
            };
            var response = dispatcher.AddPingUnitTest(account.Id, data);

            var result = response.Data;

            // Проверим параметры созданной проверки
            Assert.Equal(data.ComponentId, result.ComponentId);
            Assert.Equal(data.SystemName, result.SystemName);
            Assert.NotNull(result.DisplayName);
            Assert.Equal(data.PeriodSeconds, result.PeriodSeconds);
            Assert.Equal(data.ErrorColor, result.ErrorColor); 
            Assert.Equal(data.Host, result.Host);
            Assert.Equal(data.TimeoutMs, result.TimeoutMs);
            Assert.Equal(data.AttempMax, result.AttempMax);
        }

        [Fact]
        public void AddHttpCheckTest()
        {
            // Добавим проверку http
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            var data = new AddHttpUnitTestRequestData()
            {
                ComponentId = component.Id,
                SystemName = "Http_" + Guid.NewGuid(),
                PeriodSeconds = 600,
                ErrorColor = UnitTestResult.Alarm,
                AttempMax = 3,
                Rules = new[]
                {
                    new AddHttpUnitTestRuleRequestData()
                    {
                        SortNumber = 1,
                        DisplayName = "Rule1",
                        Url = "http://" + Guid.NewGuid() + ".ru",
                        Method = HttpRequestMethod.Get,
                        ResponseCode = 200,
                        MaxResponseSize = 1024,
                        SuccessHtml = "<body>",
                        ErrorHtml = "<error>",
                        TimeoutSeconds = 5
                    }
                }
            };

            var response = dispatcher.AddHttpUnitTest(account.Id, data);
            var result = response.Data;

            // Проверим параметры созданной проверки
            Assert.Equal(data.ComponentId, result.ComponentId);
            Assert.Equal(data.SystemName, result.SystemName);
            Assert.NotNull(result.DisplayName);
            Assert.Equal(data.PeriodSeconds, result.PeriodSeconds);
            Assert.Equal(data.ErrorColor, result.ErrorColor);
            Assert.Equal(data.AttempMax, result.AttempMax);
            var rules = data.Rules;
            Assert.NotNull(rules);
            var rule = rules.SingleOrDefault(t => t.SortNumber == data.Rules[0].SortNumber);
            Assert.NotNull(rule);
            Assert.Equal(data.Rules[0].DisplayName, rule.DisplayName);
            Assert.Equal(data.Rules[0].Url, rule.Url);
            Assert.Equal(data.Rules[0].Method, rule.Method);
            Assert.Equal(data.Rules[0].ResponseCode, rule.ResponseCode);
            Assert.Equal(data.Rules[0].MaxResponseSize, rule.MaxResponseSize);
            Assert.Equal(data.Rules[0].SuccessHtml, rule.SuccessHtml);
            Assert.Equal(data.Rules[0].ErrorHtml, rule.ErrorHtml);
            Assert.Equal(data.Rules[0].TimeoutSeconds, rule.TimeoutSeconds);
        }

        [Fact]
        public void WrongUnitTestIdTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();

            var data = new SendUnitTestResultRequestData()
            {
                UnitTestId = new Guid("00000000-1111-2222-3333-444444444444"),
                Message = "Test",
                Result = UnitTestResult.Success
            };

            var response = dispatcher.SendUnitTestResult(account.Id, data);
            Assert.False(response.Success);
        }

        [Fact]
        public void WrongUnitTestTypeIdTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();
            var component = TestHelper.GetTestApplicationComponent(account);

            var data = new GetOrCreateUnitTestRequestData()
            {
                UnitTestTypeId = new Guid("00000000-1111-2222-3333-444444444444"),
                ComponentId = component.Id,
                SystemName = "UnitTest." + Guid.NewGuid()
            };

            var response = dispatcher.GetOrCreateUnitTest(account.Id, data);
            Assert.False(response.Success);
        }

        [Fact]
        public void OutdatedCheckTest()
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
            Assert.False(response.Data.HasSignal);
        }

        [Fact]
        public void OverrideActualTimeInCheckTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Создадим новый тип проверки
            var unitTestType = dispatcher.GetOrCreateUnitTestType(account.Id, new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "UnitTestType." + Guid.NewGuid()
            }).Data;

            // Создадим новую проверку с актуальностью 1 сек
            var unitTest = dispatcher.GetOrCreateUnitTest(account.Id, new GetOrCreateUnitTestRequestData()
            {
                UnitTestTypeId = unitTestType.Id,
                ComponentId = component.Id,
                SystemName = "UnitTest." + Guid.NewGuid(),
                ActualTimeSecs = 1
            }).Data;

            // Отправим зелёную проверку, актуальную 1 час
            dispatcher.SendUnitTestResult(account.Id, new SendUnitTestResultRequestData()
            {
                UnitTestId = unitTest.Id,
                Result = UnitTestResult.Success,
                ActualIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds
            }).Check();

            // Подождём 2 секунды
            Thread.Sleep(2000);

            // Получим цвет проверки
            var response = dispatcher.GetUnitTestState(account.Id, component.Id, unitTest.Id);
            response.Check();

            // Проверим, что проверка стала красной
            Assert.Equal(MonitoringStatus.Alarm, response.Data.Status);
            Assert.False(response.Data.HasSignal);
        }

        [Fact]
        public void OverrideNoSignalColorInCheckTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Создадим новый тип проверки
            var unitTestType = dispatcher.GetOrCreateUnitTestType(account.Id, new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "UnitTestType." + Guid.NewGuid()
            }).Data;

            // Создадим новую проверку и установим жёлтый цвет, если нет сигнала
            var unitTest = dispatcher.GetOrCreateUnitTest(account.Id, new GetOrCreateUnitTestRequestData()
            {
                UnitTestTypeId = unitTestType.Id,
                ComponentId = component.Id,
                SystemName = "UnitTest." + Guid.NewGuid(),
                NoSignalColor = Core.Common.ObjectColor.Yellow
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

            // Проверим, что проверка стала жёлтой
            Assert.Equal(MonitoringStatus.Warning, response.Data.Status);
        }

        [Fact]
        public void OverrideActualTimeInCheckTypeTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Создадим новый тип проверки с актуальностью 1 сек
            var unitTestType = dispatcher.GetOrCreateUnitTestType(account.Id, new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "UnitTestType." + Guid.NewGuid(),
                ActualTimeSecs = 1
            }).Data;

            // Создадим новую проверку
            var unitTest = dispatcher.GetOrCreateUnitTest(account.Id, new GetOrCreateUnitTestRequestData()
            {
                UnitTestTypeId = unitTestType.Id,
                ComponentId = component.Id,
                SystemName = "UnitTest." + Guid.NewGuid()
            }).Data;

            // Отправим зелёную проверку, актуальную 1 час
            dispatcher.SendUnitTestResult(account.Id, new SendUnitTestResultRequestData()
            {
                UnitTestId = unitTest.Id,
                Result = UnitTestResult.Success,
                ActualIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds
            }).Check();

            // Подождём 2 секунды
            Thread.Sleep(2000);

            // Получим цвет проверки
            var response = dispatcher.GetUnitTestState(account.Id, component.Id, unitTest.Id);
            response.Check();

            // Проверим, что проверка стала красной
            Assert.Equal(MonitoringStatus.Alarm, response.Data.Status);
        }

        [Fact]
        public void OverrideNoSignalColorInCheckTypeTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Создадим новый тип проверки и укажем жёлтый цвет, если нет сигнала
            var unitTestType = dispatcher.GetOrCreateUnitTestType(account.Id, new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = "UnitTestType." + Guid.NewGuid(),
                NoSignalColor = Core.Common.ObjectColor.Yellow
            }).Data;

            // Создадим новую проверку
            var unitTest = dispatcher.GetOrCreateUnitTest(account.Id, new GetOrCreateUnitTestRequestData()
            {
                UnitTestTypeId = unitTestType.Id,
                ComponentId = component.Id,
                SystemName = "UnitTest." + Guid.NewGuid()
            }).Data;

            // Отправим зелёную проверку, актуальную 1 сек
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

            // Проверим, что проверка стала жёлтой
            Assert.Equal(MonitoringStatus.Warning, response.Data.Status);
        }
        
    }
}
