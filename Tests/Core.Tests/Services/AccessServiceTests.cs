using Moq;
using Xunit;
using Zidium.Core.AccountDb;
using Zidium.Core.Common;

namespace Zidium.Core.Tests.Services
{
    public class AccessServiceTests : BaseTest
    {
        [Fact]
        public void HasApiAccessTrueForApiKeyTest()
        {
            // ARRANGE
            var secretKey = "111111";

            var accessConfiguration = new Mock<IAccessConfiguration>();
            accessConfiguration.Setup(t => t.SecretKey).Returns(secretKey);

            var apiKeyService = new Mock<IApiKeyService>();
            apiKeyService.Setup(t => t.HasKey("222222")).Returns(true);

            var accessService = new AccessService(accessConfiguration.Object, apiKeyService.Object);

            // ACT
            var result = accessService.HasApiAccess("222222");

            // ASSERT
            Assert.True(result);
        }

        [Fact]
        public void HasApiAccessTrueForSystemKeyTest()
        {
            // ARRANGE
            var secretKey = "111111";

            var accessConfiguration = new Mock<IAccessConfiguration>();
            accessConfiguration.Setup(t => t.SecretKey).Returns(secretKey);

            var apiKeyService = new Mock<IApiKeyService>();

            var accessService = new AccessService(accessConfiguration.Object, apiKeyService.Object);

            // ACT
            var result = accessService.HasApiAccess("111111");

            // ASSERT
            Assert.True(result);
        }

        [Fact]
        public void HasApiAccessFalseTest()
        {
            // ARRANGE
            var secretKey = "111111";

            var accessConfiguration = new Mock<IAccessConfiguration>();
            accessConfiguration.Setup(t => t.SecretKey).Returns(secretKey);

            var apiKeyService = new Mock<IApiKeyService>();
            apiKeyService.Setup(t => t.HasKey("222222")).Returns(true);

            var accessService = new AccessService(accessConfiguration.Object, apiKeyService.Object);

            // ACT
            var result = accessService.HasApiAccess("333333");

            // ASSERT
            Assert.False(result);
        }

        [Fact]
        public void HasSystemAccessTrueTest()
        {
            // ARRANGE
            var secretKey = "111111";

            var accessConfiguration = new Mock<IAccessConfiguration>();
            accessConfiguration.Setup(t => t.SecretKey).Returns(secretKey);

            var apiKeyService = new Mock<IApiKeyService>();

            var accessService = new AccessService(accessConfiguration.Object, apiKeyService.Object);

            // ACT
            var result = accessService.HasSystemAccess("111111");

            // ASSERT
            Assert.True(result);
        }

        [Fact]
        public void HasSystemAccessFalseTest()
        {
            // ARRANGE
            var secretKey = "111111";

            var accessConfiguration = new Mock<IAccessConfiguration>();
            accessConfiguration.Setup(t => t.SecretKey).Returns(secretKey);

            var apiKeyService = new Mock<IApiKeyService>();

            var accessService = new AccessService(accessConfiguration.Object, apiKeyService.Object);

            // ACT
            var result = accessService.HasSystemAccess("222222");

            // ASSERT
            Assert.False(result);
        }

        [Fact]
        public void HasSystemAccessFalseForApiKeyTest()
        {
            // ARRANGE
            var secretKey = "111111";

            var accessConfiguration = new Mock<IAccessConfiguration>();
            accessConfiguration.Setup(t => t.SecretKey).Returns(secretKey);

            var apiKeyService = new Mock<IApiKeyService>();
            apiKeyService.Setup(t => t.HasKey("222222")).Returns(true);

            var accessService = new AccessService(accessConfiguration.Object, apiKeyService.Object);

            // ACT
            var result = accessService.HasSystemAccess("222222");

            // ASSERT
            Assert.False(result);
        }

        [Fact]
        public void InvalidateTest()
        {
            // ARRANGE
            var accessConfiguration = new Mock<IAccessConfiguration>();
            var timeService = new Mock<ITimeService>();
            var apiKeyService = new Mock<ApiKeyService>(timeService.Object);

            var accessService = new Mock<AccessService>(accessConfiguration.Object, apiKeyService.Object);
            accessService.CallBase = true;
            var accessServiceObject = accessService.Object;

            // ACT
            apiKeyService.Object.FireOnKeyChange("111");

            // ASSERT
            accessService.Verify(t => t.Invalidate("111"));
        }
    }
}
