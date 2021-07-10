using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Metrics
{
    public class ShowModel
    {
        public MetricInfo Metric { get; set; }

        public string ConditionRed { get; set; }

        public string ConditionYellow { get; set; }

        public string ConditionGreen { get; set; }

        public ObjectColor? ElseColor { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public TimeSpan? ActualInterval { get; set; }

        public const int LastValuesCountMax = 20;

        public MetricBreadCrumbsModel MetricBreadCrumbs { get; set; }

        public MetricHistoryForRead[] Values { get; set; }

        public class MetricInfo
        {
            public Guid Id;

            public Guid MetricTypeId;

            public string DisplayName;

            public BulbInfo Bulb;

            public ComponentInfo Component;

            public double? Value;
        }

        public class BulbInfo
        {
            public MonitoringStatus Status;

            public TimeSpan Duration;

            public DateTime StartDate;

            public DateTime EndDate;

            public DateTime ActualDate;

            public int Count;
        }

        public class ComponentInfo
        {
            public Guid Id;

            public string FullName;
        }
    }
}