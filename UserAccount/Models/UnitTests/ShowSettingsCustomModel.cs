using System;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsCustomModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan ActualTime { get; set; }
        public ObjectColor NoSignalColor { get; set; }
        public Guid TypeId { get; set; }
        public string TypeDisplayName { get; set; }

        public static ShowSettingsCustomModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var unitTestType = storage.UnitTestTypes.GetOneById(unitTest.TypeId);

            var model = new ShowSettingsCustomModel()
            {
                UnitTestId = unitTest.Id,
                TypeId = unitTest.TypeId,
                NoSignalColor = unitTest.NoSignalColor ?? unitTestType.NoSignalColor ?? ObjectColor.Red
            };
            model.TypeDisplayName = unitTestType.DisplayName;
            if (unitTest.ActualTimeSecs.HasValue)
            {
                model.ActualTime = TimeSpanHelper.FromSeconds(unitTest.ActualTimeSecs).Value;
            }
            else if (unitTestType.ActualTimeSecs.HasValue)
            {
                model.ActualTime = TimeSpanHelper.FromSeconds(unitTestType.ActualTimeSecs).Value;
            }
            else
            {
                model.ActualTime = UnitTestHelper.GetDefaultActualTime();
            }
            return model;
        }
    }
}