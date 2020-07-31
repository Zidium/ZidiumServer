using System;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsSqlModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Sql { get; set; }
        public TimeSpan CommandTimeout { get; set; }
        public TimeSpan OpenConnectionTimeout { get; set; }
        public string ConnectionString { get; set; }
        public SqlRuleDatabaseProviderType Provider { get; set; }

        public static ShowSettingsSqlModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestSqlRules.GetOneByUnitTestId(unitTest.Id);
            
            return new ShowSettingsSqlModel()
            {
                UnitTestId = unitTest.Id,
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                CommandTimeout = TimeSpanHelper.FromMilliseconds(rule.CommandTimeoutMs).Value,
                OpenConnectionTimeout = TimeSpanHelper.FromMilliseconds(rule.OpenConnectionTimeoutMs).Value,
                Sql = rule.Query,
                ConnectionString = rule.ConnectionString,
                Provider = rule.Provider
            };
        }
    }
}