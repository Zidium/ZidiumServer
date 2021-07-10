using System;
using System.Threading;
using Zidium.Agent.AgentTasks;
using Xunit;
using Zidium.Storage;
using Zidium.Api.Dto;
using Microsoft.Extensions.Logging.Abstractions;

namespace Zidium.Agent.Tests
{
    public class SqlCheckProcessorTests : BaseTest
    {
        [Fact]
        public void MsSqlTest()
        {
            var rule = new UnitTestSqlRuleForRead(
                Guid.Empty,
                SqlRuleDatabaseProviderType.MsSql,
                @"Data Source=(LocalDb)\MSSQLLocalDB;",
                5000,
                60000,
                "SELECT 'warning', 'test message'"
                );
            var processor = new SqlCheckProcessor(NullLogger.Instance, new CancellationToken(), null);
            var result = processor.CheckSql(rule);
            Assert.Equal(UnitTestResult.Warning, result.ResultRequest.Result);
            Assert.Equal("test message", result.ResultRequest.Message);
        }

        [Fact]
        public void PostgreSqlTest()
        {
            var rule = new UnitTestSqlRuleForRead(
                Guid.Empty,
                SqlRuleDatabaseProviderType.PostgreSql,
                @"Host=localhost;Port=5432;Database=Zidium.Test.Config;Username=postgres;Password=12345;",
                5000,
                60000,
                "SELECT 'warning', 'test message'"
            );
            var processor = new SqlCheckProcessor(NullLogger.Instance, new CancellationToken(), null);
            var result = processor.CheckSql(rule);
            Assert.Equal(UnitTestResult.Warning, result.ResultRequest.Result);
            Assert.Equal("test message", result.ResultRequest.Message);
        }
    }
}
