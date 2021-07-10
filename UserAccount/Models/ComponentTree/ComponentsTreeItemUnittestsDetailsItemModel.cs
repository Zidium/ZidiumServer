using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class ComponentsTreeItemUnittestsDetailsItemModel
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public MonitoringStatus Status { get; set; }

        public TimeSpan StatusDuration { get; set; }

        public string Message { get; set; }
    }
}