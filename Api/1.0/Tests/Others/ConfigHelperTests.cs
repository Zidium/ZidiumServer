﻿using Xunit;
using Zidium.Api.XmlConfig;

namespace ApiTests_1._0.Others
{
    public class ConfigHelperTests
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
