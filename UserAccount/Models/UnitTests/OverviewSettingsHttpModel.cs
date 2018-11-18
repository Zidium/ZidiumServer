using System;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsHttpModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public TimeSpan Timeout { get; set; }
        public bool HasBanner { get; set; }

        public static OverviewSettingsHttpModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var http = unitTest.HttpRequestUnitTest;
            if (http == null)
            {
                throw new Exception("unittest http data is null");
            }
            HttpRequestUnitTestRule rule = http.Rules.FirstOrDefault();
            if (rule == null)
            {
                throw new Exception("http rule is null");
            }
            return new OverviewSettingsHttpModel()
            {
                UnitTestId = unitTest.Id,
                HasBanner = http.HasBanner,
                Method = rule.Method.ToString().ToUpperInvariant(),
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Timeout = TimeSpanHelper.FromSeconds(rule.TimeoutSeconds).Value,
                Url = rule.Url
            };
        }
    }
}