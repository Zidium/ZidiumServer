using Zidium.Agent.AgentTasks;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Xunit;

namespace Zidium.Core.Tests.AgentTests
{
    public class SqlCheckProcessorTests
    {
        [Fact]
        public void CommonTest()
        {
            var rule = new UnitTestSqlRule()
            {
                Query = "SELECT 'warning', 'test message'",
                CommandTimeoutMs = 60000,
                ConnectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;",
                Provider = DatabaseProviderType.MsSql,
                OpenConnectionTimeoutMs = 5000
            };
            var result = SqlCheckProcessor.CheckSql(rule);
            Assert.Equal(UnitTestResult.Warning, result.ResultRequest.Result);
            Assert.Equal("test message", result.ResultRequest.Message);
        }
    }
}
