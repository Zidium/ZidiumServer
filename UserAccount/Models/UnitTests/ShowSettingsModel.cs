using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsModel
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string SystemName { get; set; }

        public TimeSpan? Period { get; set; }

        public TimeSpan? ActualTime { get; set; }

        public bool IsSystem { get; set; }

        public ObjectColor NoSignalColor { get; set; }

        public ObjectColor OnErrorColor { get; set; }

        public Guid TypeId { get; set; }

        public static ShowSettingsModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var unitTestType = storage.UnitTestTypes.GetOneById(unitTest.TypeId);

            var model = new ShowSettingsModel()
            {
                Id = unitTest.Id,
                DisplayName = unitTest.DisplayName,
                SystemName = unitTest.SystemName,
                TypeId = unitTest.TypeId,
                IsSystem = SystemUnitTestType.IsSystem(unitTest.TypeId),
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds)
            };

            model.NoSignalColor = unitTest.NoSignalColor ?? unitTestType.NoSignalColor ?? ObjectColor.Red;
            
            model.OnErrorColor = ObjectColorHelper.Get(unitTest.ErrorColor ?? UnitTestResult.Alarm);

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