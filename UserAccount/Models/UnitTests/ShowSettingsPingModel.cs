using System;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsPingModel
    {
        public Guid UnitTestId { get; set; }
        public string Host { get; set; }
        public TimeSpan Timeout { get; set; }

        public static ShowSettingsPingModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestPingRules.GetOneByUnitTestId(unitTest.Id);
            
            return new ShowSettingsPingModel()
            {
                UnitTestId = unitTest.Id,
                Timeout = TimeSpanHelper.FromMilliseconds(rule.TimeoutMs).Value,
                Host = rule.Host
            };
        }
    }
}