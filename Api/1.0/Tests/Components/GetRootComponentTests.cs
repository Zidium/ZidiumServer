using System;
using Zidium.Api;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Components
{
    public class GetRootComponentTests : BaseTest
    {
        [Fact]
        public void UniqueRootWrapperTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root1 = client.GetRootComponentControl();
            var root2 = client.GetRootComponentControl();
            Assert.True(object.ReferenceEquals(root1, root2));
        }

        [Fact]
        public void ApiServiceTest()
        {
            var account = TestHelper.GetTestAccount();
            IComponentControl root = account.GetClient().GetRootComponentControl();
            Assert.NotNull(root);
            Assert.NotNull(root.Info);
            Assert.NotNull(root.Info.DisplayName);
            Assert.NotNull(root.Info.SystemName);
            Assert.NotNull(root.Info.Type);
            Assert.Equal("System.ComponentTypes.Root", root.Info.Type.SystemName);
            Assert.True(root.Info.Type.IsRoot());
            Assert.True(root.Info.Id != Guid.Empty);
            Assert.Null(root.Info.ParentId);
            Assert.True(root.Info.Properties.Count == 0);
        }
    }
}
