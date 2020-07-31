using System;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks;
using Xunit;
using Zidium.Storage;

namespace Zidium.Core.Tests.AgentTests
{
    public class SqlCheckProcessorTests
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
            var processor = new SqlCheckProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), null);
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
            var processor = new SqlCheckProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), null);
            var result = processor.CheckSql(rule);
            Assert.Equal(UnitTestResult.Warning, result.ResultRequest.Result);
            Assert.Equal("test message", result.ResultRequest.Message);
        }
    }
}
