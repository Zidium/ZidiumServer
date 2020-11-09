using System;
using Zidium.Core.AccountsDb;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Others
{
    public class UserSettingTests : BaseTest
    {
        [Fact]
        public void RepositorySetAndGetSettingTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var name = "UserSetting " + DateTime.Now.Ticks;
            var value = "UserSettingValue " + DateTime.Now.Ticks;
            var storage = TestHelper.GetStorage(account.Id);

            storage.UserSettings.SetValue(user.Id, name, value);

            var settingValue = storage.UserSettings.GetValue(user.Id, name);
            Assert.Equal(value, settingValue);
        }

        [Fact]
        public void ShowComponentsAsListSettingTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var storage = TestHelper.GetStorage(account.Id);

            var service = new UserSettingService(storage);
            service.ShowComponentsAsList(user.Id, false);

            var value = service.ShowComponentsAsList(user.Id);
            Assert.False(value);

            service.ShowComponentsAsList(user.Id, true);

            value = service.ShowComponentsAsList(user.Id);
            Assert.True(value);
        }

        [Fact]
        public void SendMeNewsSettingTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var storage = TestHelper.GetStorage(account.Id);

            var service = new UserSettingService(storage);
            service.SendMeNews(user.Id, false);

            var value = service.SendMeNews(user.Id);
            Assert.False(value);

            service.SendMeNews(user.Id, true);

            value = service.SendMeNews(user.Id);
            Assert.True(value);
        }

        [Fact]
        public void TimeZoneOffsetMinutesTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var storage = TestHelper.GetStorage(account.Id);

            var service = new UserSettingService(storage);
            service.TimeZoneOffsetMinutes(user.Id, 60);

            var value = service.TimeZoneOffsetMinutes(user.Id);
            Assert.Equal(60, value);

            service.TimeZoneOffsetMinutes(user.Id, 120);

            value = service.TimeZoneOffsetMinutes(user.Id);
            Assert.Equal(120, value);
        }

    }
}
