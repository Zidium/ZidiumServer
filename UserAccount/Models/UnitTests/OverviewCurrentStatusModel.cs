using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewCurrentStatusModel
    {
        public Guid UnitTestId { get; set; }
        public MonitoringStatus Status { get; set; }
        public Guid StatusId { get; set; }
        public TimeSpan StatusDuration { get; set; }
        public string DisableComment { get; set; }
        public DateTime? DisableEndTime { get; set; }

        public static OverviewCurrentStatusModel Create(UnitTest unitTest)
        {
            var model = new OverviewCurrentStatusModel();
            model.UnitTestId = unitTest.Id;
            model.Status = unitTest.Bulb.Status;
            model.StatusId = unitTest.Bulb.StatusEventId;
            model.StatusDuration = unitTest.Bulb.GetDuration(DateTime.Now);
            model.DisableComment = unitTest.DisableComment;
            model.DisableEndTime = unitTest.DisableToDate;
            return model;
        }
    }
}