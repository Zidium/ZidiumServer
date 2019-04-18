using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

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

        public static ShowSettingsModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var model = new ShowSettingsModel()
            {
                Id = unitTest.Id,
                DisplayName = unitTest.DisplayName,
                SystemName = unitTest.SystemName,
                TypeId = unitTest.TypeId,
                IsSystem = SystemUnitTestTypes.IsSystem(unitTest.TypeId),
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds)
            };

            model.NoSignalColor = unitTest.NoSignalColor ?? unitTest.Type.NoSignalColor ?? ObjectColor.Red;
            
            model.OnErrorColor = ObjectColorHelper.Get(unitTest.ErrorColor ?? Core.Api.UnitTestResult.Alarm);

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