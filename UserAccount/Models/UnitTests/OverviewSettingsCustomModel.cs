using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsCustomModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan ActualTime { get; set; }
        public ObjectColor NoSignalColor { get; set; }

        public static OverviewSettingsCustomModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var model = new OverviewSettingsCustomModel()
            {
                UnitTestId = unitTest.Id,
                NoSignalColor = unitTest.NoSignalColor ?? unitTest.Type.NoSignalColor ?? ObjectColor.Red
            };
            if (unitTest.ActualTimeSecs.HasValue)
            {
                model.ActualTime = TimeSpanHelper.FromSeconds(unitTest.ActualTimeSecs).Value;
            }
            else if (unitTest.Type.ActualTimeSecs.HasValue)
            {
                model.ActualTime = TimeSpanHelper.FromSeconds(unitTest.Type.ActualTimeSecs).Value;
            }
            else
            {
                model.ActualTime = UnitTest.GetDefaultActualTime();
            }
            return model;
        }
    }
}