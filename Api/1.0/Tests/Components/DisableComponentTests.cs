using System;
using Xunit;
using Zidium.Api;
using Zidium.TestTools;

namespace ApiTests_1._0.Components
{
    public class DisableComponentTests
    {
        [Fact]
        public void DisableTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            var goodDate = DateTime.Now.AddDays(1);
            var goodComment = "test comment 1";

            // неверная дата
            var invalidDate = DateTime.Now.AddMinutes(-10);
            var response = component.Disable(goodComment, invalidDate);
            Assert.False(response.Success);

            // неверный комментарий
            var invalidComment = new string('d', 1001);
            response = component.Disable(invalidComment, goodDate);
            Assert.False(response.Success);

            // проверим статус
            var status = component.GetTotalState(false).Data;
            Assert.Equal(MonitoringStatus.Unknown, status.Status);

            // верный параметры
            response = component.Disable(goodComment, goodDate);
            Assert.True(response.Success);

            // проверим статус
            status = component.GetTotalState(false).Data;
            Assert.Equal(MonitoringStatus.Disabled, status.Status);
            Assert.Equal(goodComment, status.DisableComment);

            // включим
            component.Enable().Check();

            // проверим статус
            status = component.GetTotalState(false).Data;
            Assert.Equal(MonitoringStatus.Unknown, status.Status);
            Assert.Equal(goodComment, status.DisableComment);
            TestHelper.CheckDateTimesEqualBySeconds(goodDate, status.DisableToDate);

            // выключим другим методом (без даты)
            var goodComment2 = "test comment 2";
            response = component.Disable(goodComment2);
            Assert.True(response.Success);

            // проверим статус
            status = component.GetTotalState(false).Data;
            Assert.Equal(MonitoringStatus.Disabled, status.Status);
            account.SaveAllCaches();
            //account.CheckCacheNoLocks();
            Assert.Equal(goodComment2, status.DisableComment);
            Assert.Null(status.DisableToDate);
        }
    }
}
