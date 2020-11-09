using System.Web.Mvc;
using Xunit;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Tests
{
    public class WebAccountsTests : BaseTest
    {
        [Fact]
        public void AccountInfoTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);

            ShowAccountModel model;
            using (var controller = new AccountInfoController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index();
                model = (ShowAccountModel)result.Model;
            }

            Assert.Equal(account.Id, model.Id);
        }
    }
}
