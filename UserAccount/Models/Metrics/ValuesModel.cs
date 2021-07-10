using System;
using Zidium.Api.Dto;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.Counters
{
    public class ValuesModel
    {
        public Guid? ComponentId { get; set; }

        public Guid? MetricTypeId { get; set; }

        public ColorStatusSelectorValue Color { get; set; }

        public MetricInfo[] Items { get; set; }

        public class MetricInfo
        {
            public Guid Id;

            public double? Value;

            public MonitoringStatus Status;

            public string DisplayName;

            public DateTime BeginDate;

            public DateTime ActualDate;

            public Guid ComponentId;

            public ComponentBreadCrumbsModel ComponentBreadCrumbs;

        }
    }
}