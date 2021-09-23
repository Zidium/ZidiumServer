using System;
using Xunit;
using Zidium.Common;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Components
{
    public class GetOrCreateFolderTests : BaseTest
    {
        [Fact]
        public void Test()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();

            // создадим новый (системное имя уникально)
            var createFolderData = TestHelper.GetRandomGetOrCreateFolderData();
            var component = root.GetOrCreateChildFolderControl(createFolderData);
            Assert.NotNull(component);
            Assert.True(component.Info.Type.IsFolder);
            Assert.True(component.Type.Info.IsFolder);
            TestHelper.CheckComponent(createFolderData, component);

            // проверим, что повторный вызов не создает дубля
            account = TestHelper.GetTestAccount();
            root = account.GetClient().GetRootComponentControl();
            var component2 = root.GetOrCreateChildFolderControl(createFolderData.SystemName);
            TestHelper.CheckComponent(component, component2);
        }

        [Fact]
        public void CreateFolderAsTsdDo()
        {
            var x = Ulid.NewUlid().ToString();
            
            //var config = ConfigHelper.LoadFromXmlOrGetDefault();
            //config.Access.AccountId = new Guid("8BA14E0D-CEE8-4FEC-B6A0-C7B765F2D6B1");
            //config.Access.SecretKey = "fb210f14-ea97-471b-b586-b179140d311e";
            //var client = new Client("TSD", config);

            
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            

            var root = client.GetRootComponentControl();

            var tsdFolderComponent = root.GetOrCreateChildFolderControl(
                new GetOrCreateFolderData("TSD.Mobile.Devices " + x)
                {
                    DisplayName = "Терминалы с TSD " + x
                });
            var t = tsdFolderComponent.IsFake();
            Assert.False(t);
        }
    }
}
