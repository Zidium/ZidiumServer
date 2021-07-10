using System;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class CreateMetricRequestData
    {
        public Guid ComponentId { get; set; }

        public string MetricName { get; set; }

        public string AlarmCondition { get; set; }

        public string WarningCondition { get; set; }

        public string SuccessCondition { get; set; }

        public ObjectColor? ElseColor { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public int? ActualTimeSecs { get; set; }
    }
}
