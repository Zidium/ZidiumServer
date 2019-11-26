using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

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
        public DatabaseProviderType Provider { get; set; }

        public static ShowSettingsSqlModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var rule = unitTest.SqlRule;
            if (rule == null)
            {
                throw new Exception("unittest sql rule is null");
            }
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