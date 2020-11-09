using Xunit;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Tests.Others
{
    public class BannerHelperTests : BaseTest
    {
        [Fact]
        public void MainTest()
        {
            Assert.False(BannerHelper.FindBanner("http://mail.ru"));
            Assert.True(BannerHelper.FindBanner("http://fakesite.zidium.net/"));
        }
    }
}
