using System;
using Xunit;
using Zidium.Common;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Components
{
    public class FakeComponentControlTests : BaseTest
    {
        [Fact]
        public void OfflineSendEventTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            client.Config.Access.WaitOnError = TimeSpan.Zero;
            var offlineService = new FakeApiService();
            client.SetApiService(offlineService);

            var root = client.GetRootComponentControl();
            Assert.True(root.IsFake());

            var eventObj = root.CreateComponentEvent(Ulid.NewUlid().ToString());
            var response = eventObj.Send();
            Assert.False(response.Success);
        }

        [Fact]
        public void FakeCreateAndSendApplicationErrorTest()
        {
            var control = new FakeComponentControl("Fake");
            Assert.True(control.IsFake());
            var eventObj = control.CreateApplicationError(Ulid.NewUlid().ToString());
            var response = eventObj.Send();
            Assert.False(response.Success);
        }

        [Fact]
        public void FakeCreateAndAddApplicationErrorTest()
        {
            var control = new FakeComponentControl("Fake");
            Assert.True(control.IsFake());
            var eventObj = control.CreateApplicationError(Ulid.NewUlid().ToString());
            var result = eventObj.Add();
            Assert.NotNull(result);
        }

        [Fact]
        public void FakeSendComponentEventTest()
        {
            var control = new FakeComponentControl("Fake");
            Assert.True(control.IsFake());
            var eventObj = control.CreateComponentEvent(Ulid.NewUlid().ToString());
            var response = eventObj.Send();
            Assert.False(response.Success);
        }

        [Fact]
        public void FakeAddApplicationErrorTest()
        {
            var control = new FakeComponentControl("Fake");
            Assert.True(control.IsFake());
            var eventObj = control.AddApplicationError(Ulid.NewUlid().ToString(), string.Empty);
            Assert.NotNull(eventObj);
        }

        [Fact]
        public void FakeAddComponentEventTest()
        {
            var control = new FakeComponentControl("Fake");
            Assert.True(control.IsFake());
            var eventObj = control.AddComponentEvent(Ulid.NewUlid().ToString(), string.Empty);
            Assert.NotNull(eventObj);
        }

        [Fact]
        public void FakeGetOrCreateChildComponentControlTest()
        {
            var control = new FakeComponentControl("Fake");
            var child = control.GetOrCreateChildComponentControl(Ulid.NewUlid().ToString(), Ulid.NewUlid().ToString());
            Assert.NotNull(child);
            Assert.True(child.IsFake());
        }

        [Fact]
        public void FakeGetOrCreateUnitTestControlTest()
        {
            var control = new FakeComponentControl("Fake");
            var unitTestControl = control.GetOrCreateUnitTestControl(Ulid.NewUlid().ToString());
            Assert.NotNull(unitTestControl);
            Assert.True(unitTestControl.IsFake());
        }

        [Fact]
        public void FakeGetLogTest()
        {
            var control = new FakeComponentControl("Fake");
            var logControl = control.Log;
            Assert.NotNull(logControl);
            Assert.True(logControl.IsFake());
        }

        [Fact]
        public void FakeTypeTest()
        {
            var control = new FakeComponentControl("Fake");
            var type = control.Type;
            Assert.NotNull(type);
            Assert.True(type.IsFake());
        }
    }
}
