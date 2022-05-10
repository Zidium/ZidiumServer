using System;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

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

        public static OverviewCurrentStatusModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            var bulb = storage.Bulbs.GetOneById(unitTest.StatusDataId);
            var model = new OverviewCurrentStatusModel();
            model.UnitTestId = unitTest.Id;
            model.Status = bulb.Status;
            model.StatusId = bulb.StatusEventId;
            model.StatusDuration = bulb.GetDuration(DateTime.UtcNow);
            model.DisableComment = unitTest.DisableComment;
            model.DisableEndTime = unitTest.DisableToDate;
            return model;
        }
    }
}