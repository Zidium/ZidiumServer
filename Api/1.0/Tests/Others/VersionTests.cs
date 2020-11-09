using Zidium.Api.Others;
using Xunit;

namespace ApiTests_1._0.Others
{
    public class VersionTests : BaseTest
    {
        [Fact]
        public void Test()
        {
            Assert.Null(PrepareDataHelper.FixVersion("999999999999"));
            Assert.Null(PrepareDataHelper.FixVersion("1.a"));
            Assert.Null(PrepareDataHelper.FixVersion("a.b.c.d"));
            Assert.Null(PrepareDataHelper.FixVersion("abcd"));

            Assert.Equal("0.0", PrepareDataHelper.FixVersion("0"));
            Assert.Equal("1.0", PrepareDataHelper.FixVersion("1"));
            Assert.Equal("15.0", PrepareDataHelper.FixVersion("15"));
            Assert.Equal("5.60", PrepareDataHelper.FixVersion("5.60"));
            Assert.Equal("5.60.774", PrepareDataHelper.FixVersion("5.60.774"));
            Assert.Equal("10.10.10.10", PrepareDataHelper.FixVersion("10.10.10.10"));
            Assert.Equal("100.100.100.100", PrepareDataHelper.FixVersion("100.100.100.100"));
        }
    }
}
