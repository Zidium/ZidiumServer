using System;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;

namespace Zidium.Core.Tests.Others
{
    public class UrlHelperTests
    {
        [Fact]
        public void GetAccountNameFromUrlTest()
        {
            var result = UrlHelper.GetAccountNameFromUrl("http://test.lk.zidium.net", "http://lk.zidium.net");
            Assert.Equal("test", result);
        }

        [Fact]
        public void GetAccountNameFromLocalhostUrlTest()
        {
            var result = UrlHelper.GetAccountNameFromUrl("http://localhost", "http://lk.zidium.net");
            Assert.Null(result);
        }

        [Fact]
        public void AddAccountNameToUrlTest()
        {
            var result = UrlHelper.GetFullUrl("test", "/Components/1", "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/Components/1", result);
        }

        [Fact]
        public void GetAccountWebsiteUrlTest()
        {
            var result = UrlHelper.GetAccountWebsiteUrl("test", "/Info", "http://zidium.net/Logon", "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/Info", result);
        }

        [Fact]
        public void GetAccountWebsiteUrlHttpsTest()
        {
            var result = UrlHelper.GetAccountWebsiteUrl("test", "/Info", "https://zidium.net/Logon", "http://lk.zidium.net");
            Assert.Equal("https://lk.zidium.net/Info", result);
        }

        [Fact]
        public void GetAccountLocalhostUrlTest()
        {
            var result = UrlHelper.GetAccountWebsiteUrl("test", "/Info", "http://localhost/Logon", "http://lk.zidium.net");
            Assert.Equal("http://localhost:16997/Info", result);
        }

        [Fact]
        public void GetEventUrlTest()
        {
            var result = UrlHelper.GetEventUrl(new Guid("e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f"), "test", "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/Events/e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f", result);
        }

        [Fact]
        public void GetComponentUrlTest()
        {
            var result = UrlHelper.GetComponentUrl(new Guid("e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f"), "test", "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/Components/e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f", result);
        }

        [Fact]
        public void GetUnitTestUrlTest()
        {
            var result = UrlHelper.GetUnitTestUrl(new Guid("e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f"), "test", "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/UnitTests/ResultDetails/e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f", result);
        }

        [Fact]
        public void GetSubscriptionEditUrlTest()
        {
            var user = new User()
            {
                Id = Guid.NewGuid()
            };
            var component = new Component()
            {
                Id = Guid.NewGuid()
            };
            var result = UrlHelper.GetSubscriptionEditUrl(component, user, "test", "http://lk.zidium.net");
            var url = $"http://lk.zidium.net/Subscriptions/EditComponentSubscriptions?componentId={component.Id}&userId={user.Id}";
            Assert.Equal(url, result);
        }

        [Fact]
        public void GetPasswordSetUrlTest()
        {
            var result = UrlHelper.GetPasswordSetUrl(new Guid("37ee84be-2365-443b-9c82-a6553efc5782"), new Guid("e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f"), "test", "http://lk.zidium.net");
            Assert.Equal("http://lk.zidium.net/Home/SetPassword/e0bf1fb0-3f40-47c8-ab52-3e141df9cd6f?accountId=37ee84be-2365-443b-9c82-a6553efc5782", result);
        }

    }
}
