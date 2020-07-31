using System;
using System.Linq;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

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

        public static OverviewSettingsHttpModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var httpUnitTest = storage.HttpRequestUnitTests.GetOneByUnitTestId(unitTest.Id);

            var rule = storage.HttpRequestUnitTestRules.GetByUnitTestId(unitTest.Id).FirstOrDefault();
            if (rule == null)
            {
                throw new Exception("http rule is null");
            }

            return new OverviewSettingsHttpModel()
            {
                UnitTestId = unitTest.Id,
                HasBanner = httpUnitTest.HasBanner,
                Method = rule.Method.ToString().ToUpperInvariant(),
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Timeout = TimeSpanHelper.FromSeconds(rule.TimeoutSeconds).Value,
                Url = rule.Url
            };
        }
    }
}