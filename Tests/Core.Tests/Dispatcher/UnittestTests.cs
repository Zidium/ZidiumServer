using System;
using System.Linq;
using System.Threading;
using Xunit;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class UnittestTests
    {
        [Fact]
        public void CreateUnittestWithoutTypeTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();
            var component = account.CreateTestApplicationComponent();

            var data = new GetOrCreateUnitTestRequestData()
            {
                ComponentId = component.Id,
                UnitTestTypeId = null,
                SystemName = "Unittest." + Guid.NewGuid()
            };

            var unittest = dispatcher.GetOrCreateUnitTest(account.Id, data).Data;
            var unittestType = dispatcher.GetUnitTestTypeById(account.Id, unittest.TypeId).Data;
            Assert.Equal("CustomUnitTestType", unittestType.SystemName);
        }

        [Fact]
        public void SendUnitTestResultsTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();
            var component = account.CreateTestApplicationComponent();
            var unittest1 = TestHelper.CreateTestUnitTest(account.Id, component.Id);
            var unittest2 = TestHelper.CreateTestUnitTest(account.Id, component.Id);

            var data = new[]
            {
                new SendUnitTestResultRequestData()
                {
                    UnitTestId = unittest1.Id,
                    Result = UnitTestResult.Success
                },
                new SendUnitTestResultRequestData()
                {
                    UnitTestId = unittest2.Id,
                    Result = UnitTestResult.Warning
                }
            };

            dispatcher.SendUnitTestResults(account.Id, data);

            var status1 = dispatcher.GetUnitTestState(account.Id, component.Id, unittest1.Id).Data.Status;
            var status2 = dispatcher.GetUnitTestState(account.Id, component.Id, unittest2.Id).Data.Status;

            Assert.Equal(MonitoringStatus.Success, status1);
            Assert.Equal(MonitoringStatus.Warning, status2);
        }

        [Fact]
        public void ChangeNoSignalColorTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // �������� ����� ��� �������� � ����� ��������
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

            // �������� ������ ��������, ���������� 1 �������
            dispatcher.SendUnitTestResult(account.Id, new SendUnitTestResultRequestData()
            {
                UnitTestId = unitTest.Id,
                Result = UnitTestResult.Success,
                ActualIntervalSeconds = 1
            }).Check();

            // ������� 2 �������
            Thread.Sleep(2000);

            // ������� ���� ��������
            var response = dispatcher.GetUnitTestState(account.Id, component.Id, unitTest.Id);
            response.Check();

            // ��������, ��� �������� ����� �������
            Assert.Equal(MonitoringStatus.Alarm, response.Data.Status);

            // �������� "���� ���� ��� �������" �� �����
            dispatcher.UpdateUnitTest(account.Id, new UpdateUnitTestRequestData()
            {
                UnitTestId = unitTest.Id,
                DisplayName = unitTest.DisplayName,
                NoSignalColor = ObjectColor.Yellow
            }).Check();

            // ������� ���� ��������
            response = dispatcher.GetUnitTestState(account.Id, component.Id, unitTest.Id);
            response.Check();

            // ��������, ��� �������� ����� �����
            Assert.Equal(MonitoringStatus.Warning, response.Data.Status);
            Assert.False(response.Data.HasSignal);
        }
    }
}
