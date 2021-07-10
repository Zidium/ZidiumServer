using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class ComponentDetailsStateModel
    {
        public Guid Id { get; set; }

        public string SystemName { get; set; }

        public MonitoringStatus Status { get; set; }

        public Guid StatusEventId { get; set; }

        public TimeSpan StatusDuration { get; set; }

        public bool CanEdit { get; set; }

        public bool IsEnabled { get; set; }

        public ComponentMiniStatusModel EventsMiniStatus { get; set; }

        public ComponentMiniStatusModel UnittestsMiniStatus { get; set; }

        public ComponentMiniStatusModel MetricsMiniStatus { get; set; }

        public ComponentMiniStatusModel ChildsMiniStatus { get; set; }

    }
}