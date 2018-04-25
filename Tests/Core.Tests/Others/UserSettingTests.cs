using System;
using Zidium.Core.AccountsDb;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Others
{
    public class UserSettingTests
    {
        [Fact]
        public void RepositorySetAndGetSettingTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var name = "UserSetting " + DateTime.Now.Ticks;
            var value = "UserSettingValue " + DateTime.Now.Ticks;
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var repository = accountContext.GetUserSettingRepository();
                repository.SetValue(user.Id, name, value);
                accountContext.SaveChanges();
            }
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var repository = accountContext.GetUserSettingRepository();
                var settingValue = repository.GetValue(user.Id, name);
                Assert.Equal(value, settingValue);
            }
        }

        [Fact]
        public void ShowComponentsAsListSettingTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var service = accountContext.GetUserSettingService();
                service.ShowComponentsAsList(user.Id, false);
                accountContext.SaveChanges();
            }
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var service = accountContext.GetUserSettingService();
                var value = service.ShowComponentsAsList(user.Id);
                Assert.False(value);
            }
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var service = accountContext.GetUserSettingService();
                service.ShowComponentsAsList(user.Id, true);
                accountContext.SaveChanges();
            }
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var service = accountContext.GetUserSettingService();
                var value = service.ShowComponentsAsList(user.Id);
                Assert.True(value);
            }
        }

        [Fact]
        public void SendMeNewsSettingTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var service = accountContext.GetUserSettingService();
                service.SendMeNews(user.Id, false);
                accountContext.SaveChanges();
            }
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var service = accountContext.GetUserSettingService();
                var value = service.SendMeNews(user.Id);
                Assert.False(value);
            }
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var service = accountContext.GetUserSettingService();
                service.SendMeNews(user.Id, true);
                accountContext.SaveChanges();
            }
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var service = accountContext.GetUserSettingService();
                var value = service.SendMeNews(user.Id);
                Assert.True(value);
            }
        }

    }
}
