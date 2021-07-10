using System;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsVirusTotalModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Url { get; set; }

        public static OverviewSettingsVirusTotalModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestVirusTotalRules.GetOneByUnitTestId(unitTest.Id);
            
            return new OverviewSettingsVirusTotalModel()
            {
                UnitTestId = unitTest.Id,
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Url = rule.Url
            };
        }
    }
}