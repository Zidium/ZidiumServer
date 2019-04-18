using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsCustomModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan ActualTime { get; set; }
        public ObjectColor NoSignalColor { get; set; }
        public Guid TypeId { get; set; }
        public string TypeDisplayName { get; set; }

        public static ShowSettingsCustomModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var model = new ShowSettingsCustomModel()
            {
                UnitTestId = unitTest.Id,
                TypeId = unitTest.TypeId,
                NoSignalColor = unitTest.NoSignalColor ?? unitTest.Type.NoSignalColor ?? ObjectColor.Red
            };
            model.TypeDisplayName = unitTest.Type.DisplayName;
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
                model.ActualTime = UnitTestHelper.GetDefaultActualTime();
            }
            return model;
        }
    }
}