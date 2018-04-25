using System;
using Zidium.Core.Common;

namespace Zidium.Core.Api
{
    public class UpdateMetricRequestData
    {
        public Guid MetricId { get; set; }

        public string AlarmCondition { get; set; }

        public string WarningCondition { get; set; }

        public string SuccessCondition { get; set; }

        public ObjectColor? ElseColor { get; set; }

        public int? ActualTimeSecs { get; set; }

        public ObjectColor? NoSignalColor { get; set; }
    }
}
