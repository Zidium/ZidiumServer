using System;
using System.Linq;
using Xunit;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class ApiKeysTests : BaseTest
    {
        [Fact]
        public void GetApiKeysTest()
        {
            // ARRANGE
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser();

            var apiKey1 = new DbApiKey()
            {
                Id = Ulid.NewUlid(),
                Name = "Api key 1",
                Value = Guid.NewGuid().ToString(),
                UpdatedAt = new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc)
            };

            var apiKey2 = new DbApiKey()
            {
                Id = Ulid.NewUlid(),
                Name = "Api key 2",
                Value = Guid.NewGuid().ToString(),
                UpdatedAt = new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc),
                UserId = user.Id
            };

            using (var dbContext = account.GetDbContext())
            {
                dbContext.ApiKeys.Add(apiKey1);
                dbContext.ApiKeys.Add(apiKey2);
                dbContext.SaveChanges();
            }

            var dispatcher = TestHelper.GetDispatcherClient();

            // ACT
            var result = dispatcher.GetApiKeys();

            // ASSERT
            Assert.NotNull(result);
            result.Check();

            var resultKey1 = result.Data.FirstOrDefault(t => t.Id == apiKey1.Id);
            Assert.NotNull(resultKey1);
            Assert.Equal("Api key 1", resultKey1.Name);
            Assert.Equal(new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc), resultKey1.UpdatedAt);
            Assert.Null(resultKey1.UserId);
            Assert.Null(resultKey1.User);

            var resultKey2 = result.Data.FirstOrDefault(t => t.Id == apiKey2.Id);
            Assert.NotNull(resultKey2);
            Assert.Equal("Api key 2", resultKey2.Name);
            Assert.Equal(new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc), resultKey2.UpdatedAt);
            Assert.Equal(user.Id, resultKey2.UserId);
            Assert.Equal(user.NameOrLogin(), resultKey2.User);
        }

        [Fact]
        public void AddApiKeyTest()
        {
            // ARRANGE
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser();

            var dispatcher = TestHelper.GetDispatcherClient();

            var data = new AddApiKeyRequestData()
            {
                Id = Ulid.NewUlid(),
                Name = "New api key",
                Value = Guid.NewGuid().ToString(),
                UserId = user.Id
            };

            // ACT
            dispatcher.AddApiKey(data);

            // ASSERT
            using (var dbContext = account.GetDbContext())
            {
                var apiKey = dbContext.ApiKeys.Find(data.Id);
                Assert.NotNull(apiKey);
                Assert.Equal("New api key", apiKey.Name);
                Assert.Equal(data.Value, apiKey.Value);
                Assert.Equal(user.Id, apiKey.UserId);
            }
        }

        [Fact]
        public void GetApiKeyByIdTest()
        {
            // ARRANGE
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser();

            var apiKey = new DbApiKey()
            {
                Id = Ulid.NewUlid(),
                Name = "Api key 1",
                Value = Guid.NewGuid().ToString(),
                UserId = user.Id,
                UpdatedAt = new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc)
            };

            using (var dbContext = account.GetDbContext())
            {
                dbContext.ApiKeys.Add(apiKey);
                dbContext.SaveChanges();
            }

            var dispatcher = TestHelper.GetDispatcherClient();

            // ACT
            var result = dispatcher.GetApiKeyById(apiKey.Id);

            // ASSERT
            Assert.NotNull(result);
            result.Check();

            var resultKey = result.Data;
            Assert.NotNull(resultKey);
            Assert.Equal("Api key 1", resultKey.Name);
            Assert.Equal(apiKey.Value, resultKey.Value);
            Assert.Equal(new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc), resultKey.UpdatedAt);
            Assert.Equal(user.Id, resultKey.UserId);
            Assert.Equal(user.NameOrLogin(), resultKey.User);
        }

        [Fact]
        public void UpdateApiKeyTest()
        {
            // ARRANGE
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser();

            var existingApiKey = new DbApiKey()
            {
                Id = Ulid.NewUlid(),
                Name = "Api key 1",
                Value = Guid.NewGuid().ToString(),
                UserId = null,
                UpdatedAt = new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc)
            };

            using (var dbContext = account.GetDbContext())
            {
                dbContext.ApiKeys.Add(existingApiKey);
                dbContext.SaveChanges();
            }

            var dispatcher = TestHelper.GetDispatcherClient();

            var data = new UpdateApiKeyRequestData()
            {
                Id = existingApiKey.Id,
                Name = "Api key 2",
                UserId = user.Id
            };

            // ACT
            dispatcher.UpdateApiKey(data);

            // ASSERT
            using (var dbContext = account.GetDbContext())
            {
                var apiKey = dbContext.ApiKeys.Find(data.Id);
                Assert.NotNull(apiKey);
                Assert.Equal("Api key 2", apiKey.Name);
                Assert.Equal(existingApiKey.Value, apiKey.Value);
                Assert.Equal(user.Id, apiKey.UserId);
            }
        }

        [Fact]
        public void DeleteApiKeyTest()
        {
            // ARRANGE
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser();

            var existingApiKey = new DbApiKey()
            {
                Id = Ulid.NewUlid(),
                Name = "Api key 1",
                Value = Guid.NewGuid().ToString(),
                UserId = user.Id,
                UpdatedAt = new DateTime(2022, 12, 31, 20, 0, 0, DateTimeKind.Utc)
            };

            using (var dbContext = account.GetDbContext())
            {
                dbContext.ApiKeys.Add(existingApiKey);
                dbContext.SaveChanges();
            }

            var dispatcher = TestHelper.GetDispatcherClient();

            // ACT
            dispatcher.DeleteApiKey(existingApiKey.Id);

            // ASSERT
            using (var dbContext = account.GetDbContext())
            {
                var apiKey = dbContext.ApiKeys.Find(existingApiKey.Id);
                Assert.Null(apiKey);
            }
        }
    }
}
