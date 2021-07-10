using System;
using Zidium.Api.Dto;
using Zidium.UserAccount.Models.ExtentionProperties;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class UnittestDetailsStateModel
    {
        public Guid Id { get; set; }

        public MonitoringStatus Status { get; set; }

        public Guid StatusEventId { get; set; }

        public TimeSpan StatusDuration { get; set; }

        public bool CanRun { get; set; }

        public bool CanEdit { get; set; }

        public bool IsEnabled { get; set; }

        public Guid TypeId { get; set; }

        public string RuleData { get; set; }

        public DateTime? LastExecutionDate { get; set; }

        public string LastExecutionResult { get; set; }

        public ExtentionPropertiesModel LastExecutionResultProperties { get; set; }
    }
}