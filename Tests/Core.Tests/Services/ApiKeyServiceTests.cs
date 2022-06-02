using System;
using System.Linq;
using Moq;
using Xunit;
using Zidium.Common;
using Zidium.Core.AccountDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Services
{
    public class ApiKeyServiceTests : BaseTest
    {
        [Fact]
        public void HasKeyTrueTest()
        {
            // ARRANGE
            var storage = TestHelper.GetStorage();

            var keyValue = Guid.NewGuid().ToString();
            var apiKey = new ApiKeyForAdd()
            {
                Name = "Test key",
                Value = keyValue,
                UpdatedAt = DateTime.UtcNow
            };
            storage.ApiKeys.Add(apiKey);

            var timeService = new Mock<ITimeService>();

            var service = new ApiKeyService(timeService.Object);
            service.Load(storage);

            // ACT
            var result = service.HasKey(keyValue);

            // ASSERT
            Assert.True(result);
        }

        [Fact]
        public void HasKeyFalseTest()
        {
            // ARRANGE
            var storage = TestHelper.GetStorage();

            var keyValue = Guid.NewGuid().ToString();

            var timeService = new Mock<ITimeService>();

            var service = new ApiKeyService(timeService.Object);
            service.Load(storage);

            // ACT
            var result = service.HasKey(keyValue);

            // ASSERT
            Assert.False(result);
        }

        [Fact]
        public void GetAllTest()
        {
            // ARRANGE
            var storage = TestHelper.GetStorage();
            var user = TestHelper.CreateTestUser();

            var keyValue = Guid.NewGuid().ToString();
            var apiKey = new ApiKeyForAdd()
            {
                Id = Ulid.NewUlid(),
                Name = "Test key",
                Value = keyValue,
                UpdatedAt = new DateTime(2022, 12, 01, 20, 00, 00, DateTimeKind.Utc),
                UserId = user.Id
            };
            storage.ApiKeys.Add(apiKey);

            var timeService = new Mock<ITimeService>();

            var service = new ApiKeyService(timeService.Object);
            service.Load(storage);

            // ACT
            var result = service.GetAll();

            // ASSERT
            var resultKey = result.FirstOrDefault(t => t.Id == apiKey.Id);
            Assert.NotNull(resultKey);

            Assert.Equal("Test key", resultKey.Name);
            Assert.Equal(keyValue, resultKey.Value);
            Assert.Equal(new DateTime(2022, 12, 01, 20, 00, 00, DateTimeKind.Utc), resultKey.UpdatedAt);
            Assert.Equal(user.Id, resultKey.UserId);
        }

        [Fact]
        public void AddNonExistingTest()
        {
            // ARRANGE
            var storage = TestHelper.GetStorage();
            var user = TestHelper.CreateTestUser();

            var now = new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc);
            var timeService = new Mock<ITimeService>();
            timeService.Setup(t => t.Now()).Returns(now);

            var data = new AddApiKeyRequestData()
            {
                Id = Ulid.NewUlid(),
                Name = "New api key",
                Value = Guid.NewGuid().ToString(),
                UserId = user.Id
            };

            bool onChangeEventFired = false;

            var service = new ApiKeyService(timeService.Object);
            service.Load(storage);
            service.OnKeyChange += (key) =>
            {
                if (key == data.Value)
                {
                    onChangeEventFired = true;
                }
            };

            // ACT
            service.Add(storage, data);

            // ASSERT
            Assert.True(service.HasKey(data.Value));
            Assert.True(onChangeEventFired);

            var apiKey = storage.ApiKeys.GetOneById(data.Id);
            Assert.Equal("New api key", apiKey.Name);
            Assert.Equal(data.Value, apiKey.Value);
            Assert.Equal(now, apiKey.UpdatedAt);
            Assert.Equal(user.Id, apiKey.UserId);
        }

        [Fact]
        public void AddExistingTest()
        {
            // ARRANGE
            var storage = TestHelper.GetStorage();
            var user = TestHelper.CreateTestUser();

            var keyValue = Guid.NewGuid().ToString();
            var apiKey = new ApiKeyForAdd()
            {
                Id = Ulid.NewUlid(),
                Name = "Test key",
                Value = keyValue,
                UpdatedAt = new DateTime(2022, 12, 01, 20, 00, 00, DateTimeKind.Utc),
                UserId = user.Id
            };
            storage.ApiKeys.Add(apiKey);

            var now = new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc);
            var timeService = new Mock<ITimeService>();
            timeService.Setup(t => t.Now()).Returns(now);

            var data = new AddApiKeyRequestData()
            {
                Id = Ulid.NewUlid(),
                Name = "New api key",
                Value = keyValue,
                UserId = user.Id
            };

            bool onChangeEventFired = false;

            var service = new ApiKeyService(timeService.Object);
            service.Load(storage);
            service.OnKeyChange += (key) =>
            {
                if (key == data.Value)
                {
                    onChangeEventFired = true;
                }
            };

            // ACT
            Assert.Throws<UserFriendlyException>(() =>
            {
                service.Add(storage, data);
            });

            // ASSERT
            Assert.False(onChangeEventFired);
        }

        [Fact]
        public void GetByIdTest()
        {
            // ARRANGE
            var storage = TestHelper.GetStorage();
            var user = TestHelper.CreateTestUser();

            var keyValue = Guid.NewGuid().ToString();
            var apiKey = new ApiKeyForAdd()
            {
                Id = Ulid.NewUlid(),
                Name = "Test key",
                Value = keyValue,
                UpdatedAt = new DateTime(2022, 12, 01, 20, 00, 00, DateTimeKind.Utc),
                UserId = user.Id
            };
            storage.ApiKeys.Add(apiKey);

            var timeService = new Mock<ITimeService>();

            var service = new ApiKeyService(timeService.Object);
            service.Load(storage);

            // ACT
            var result = service.GetById(apiKey.Id);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("Test key", result.Name);
            Assert.Equal(keyValue, result.Value);
            Assert.Equal(new DateTime(2022, 12, 01, 20, 00, 00, DateTimeKind.Utc), result.UpdatedAt);
            Assert.Equal(user.Id, result.UserId);
        }

        [Fact]
        public void UpdateTest()
        {
            // ARRANGE
            var storage = TestHelper.GetStorage();
            var user = TestHelper.CreateTestUser();

            var keyValue = Guid.NewGuid().ToString();
            var apiKey = new ApiKeyForAdd()
            {
                Id = Ulid.NewUlid(),
                Name = "Test key",
                Value = keyValue,
                UpdatedAt = new DateTime(2022, 12, 01, 00, 00, 00, DateTimeKind.Utc),
                UserId = null
            };
            storage.ApiKeys.Add(apiKey);

            var now = new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc);
            var timeService = new Mock<ITimeService>();
            timeService.Setup(t => t.Now()).Returns(now);

            var data = new UpdateApiKeyRequestData()
            {
                Id = apiKey.Id,
                Name = "New api key 2",
                UserId = user.Id
            };

            bool onChangeEventFired = false;

            var service = new ApiKeyService(timeService.Object);
            service.Load(storage);
            service.OnKeyChange += (key) =>
            {
                if (key == apiKey.Value)
                {
                    onChangeEventFired = true;
                }
            };

            // ACT
            service.Update(storage, data);

            // ASSERT
            Assert.True(onChangeEventFired);

            var updatedApiKey = storage.ApiKeys.GetOneById(data.Id);
            Assert.Equal("New api key 2", updatedApiKey.Name);
            Assert.Equal(apiKey.Value, updatedApiKey.Value);
            Assert.Equal(now, updatedApiKey.UpdatedAt);
            Assert.Equal(user.Id, updatedApiKey.UserId);
        }

        [Fact]
        public void DeleteTest()
        {
            // ARRANGE
            var storage = TestHelper.GetStorage();
            var user = TestHelper.CreateTestUser();

            var keyValue = Guid.NewGuid().ToString();
            var apiKey = new ApiKeyForAdd()
            {
                Id = Ulid.NewUlid(),
                Name = "Test key",
                Value = keyValue,
                UpdatedAt = new DateTime(2022, 12, 01, 00, 00, 00, DateTimeKind.Utc),
                UserId = user.Id
            };
            storage.ApiKeys.Add(apiKey);

            var timeService = new Mock<ITimeService>();

            bool onChangeEventFired = false;

            var service = new ApiKeyService(timeService.Object);
            service.Load(storage);
            service.OnKeyChange += (key) =>
            {
                if (key == apiKey.Value)
                {
                    onChangeEventFired = true;
                }
            };

            // ACT
            service.Delete(storage, apiKey.Id);

            // ASSERT
            Assert.False(service.HasKey(keyValue));
            Assert.True(onChangeEventFired);

            var deletedApiKey = storage.ApiKeys.GetOneOrNullById(apiKey.Id);
            Assert.Null(deletedApiKey);
        }
    }
}
