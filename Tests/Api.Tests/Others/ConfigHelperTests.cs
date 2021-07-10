using Xunit;
using Zidium.Api.XmlConfig;

namespace Zidium.Api.Tests.Others
{
    public class ConfigHelperTests : BaseTest
    {
        [Fact]
        public void LoadFromNonExistingResourceTest()
        {
            var config = ConfigHelper.LoadFromResource("zidium-non-existing.config");
            Assert.Null(config);
        }

        [Fact]
        public void LoadFromResourceTest()
        {
            var config = ConfigHelper.LoadFromResource("zidium-resource.config");
            Assert.NotNull(config);
        }
    }
}
