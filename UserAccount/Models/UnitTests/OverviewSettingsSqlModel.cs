using System;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsSqlModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Sql { get; set; }
        public TimeSpan Timeout { get; set; }

        public static OverviewSettingsSqlModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestSqlRules.GetOneByUnitTestId(unitTest.Id);
            
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