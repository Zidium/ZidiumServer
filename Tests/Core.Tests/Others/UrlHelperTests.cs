using System;
using Xunit;
using Zidium.Core.Common;

namespace Zidium.Core.Tests.Others
{
    public class UrlHelperTests
    {

        [Fact]
        public void GetAccountWebsiteUrlTest()
        {
            var result = UrlHelper.GetAccountWebsiteUrl("/Info", "http://zidium.net/Logon", "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/Info", result);
        }

        [Fact]
        public void GetAccountWebsiteUrlHttpsTest()
        {
            var result = UrlHelper.GetAccountWebsiteUrl("/Info", "https://zidium.net/Logon", "http://lk.zidium.net");
            Assert.Equal("https://lk.zidium.net/Info", result);
        }

        [Fact]
        public void GetAccountLocalhostUrlTest()
        {
            var result = UrlHelper.GetAccountWebsiteUrl("/Info", "http://localhost:16997/Logon", "http://lk.zidium.net");
            Assert.Equal("http://localhost:16997/Info", result);
        }

        [Fact]
        public void GetEventUrlTest()
        {
            var result = UrlHelper.GetEventUrl(new Guid("e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f"), "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/Events/e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f", result);
        }

        [Fact]
        public void GetComponentUrlTest()
        {
            var result = UrlHelper.GetComponentUrl(new Guid("e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f"), "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/Components/e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f", result);
        }

        [Fact]
        public void GetUnitTestUrlTest()
        {
            var result = UrlHelper.GetUnitTestUrl(new Guid("e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f"), "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/UnitTests/ResultDetails/e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f", result);
        }

        [Fact]
        public void GetSubscriptionEditUrlTest()
        {
            var userId = Guid.NewGuid();

            var componentId = Guid.NewGuid();

            var result = UrlHelper.GetSubscriptionEditUrl(componentId, userId, "http://lk.zidium.net");
            var url = $"http://lk.zidium.net/Subscriptions/EditComponentSubscriptions?componentId={componentId}&userId={userId}";
            Assert.Equal(url, result);
        }

        [Fact]
        public void GetPasswordSetUrlTest()
        {
            var result = UrlHelper.GetPasswordSetUrl(new Guid("e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f"), "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/Home/SetPassword/e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f", result);
        }

    }
}
