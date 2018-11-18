using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsSqlModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Sql { get; set; }
        public TimeSpan Timeout { get; set; }

        public static OverviewSettingsSqlModel Create(UnitTest unitTest)
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
            return new OverviewSettingsSqlModel()
            {
                UnitTestId = unitTest.Id,
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Timeout = TimeSpanHelper.FromMilliseconds(rule.CommandTimeoutMs).Value,
                Sql = rule.Query
            };
        }
    }
}