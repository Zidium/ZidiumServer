using System;
using Xunit;
using Zidium.Api;

namespace ApiTests_1._0.UnitTests
{
    public class FakeUnitTestTests
    {
        [Fact]
        public void FakeSendUnitTestResultTest()
        {
            var unitTestControl = new FakeUnitTestControl();
            var response = unitTestControl.SendResult(UnitTestResult.Success, TimeSpan.FromDays(1));
            Assert.NotNull(response);
            Assert.False(response.Success);
        }

        [Fact]
        public void FakeGetStateTest()
        {
            var unitTestControl = new FakeUnitTestControl();
            var response = unitTestControl.GetState();
            Assert.NotNull(response);
            Assert.False(response.Success);
        }

        [Fact]
        public void FakeSetEnableTest()
        {
            var unitTestControl = new FakeUnitTestControl();
            var response = unitTestControl.Enable();
            Assert.NotNull(response);
            Assert.False(response.Success);
        }

    }
}
