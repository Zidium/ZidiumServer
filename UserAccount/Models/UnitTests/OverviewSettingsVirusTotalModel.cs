using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsVirusTotalModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Url { get; set; }

        public static OverviewSettingsVirusTotalModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var rule = unitTest.VirusTotalRule;
            if (rule == null)
            {
                throw new Exception("unittest rule is null");
            }
            return new OverviewSettingsVirusTotalModel()
            {
                UnitTestId = unitTest.Id,
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Url = rule.Url
            };
        }
    }
}