using System;
using Zidium.Storage;

namespace Zidium.Core.Api
{
    public class UpdateMetricTypeRequestData
    {
        public Guid MetricTypeId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public string AlarmCondition { get; set; }

        public string WarningCondition { get; set; }

        public string SuccessCondition { get; set; }

        public ObjectColor? ElseColor { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public int? ActualTimeSecs { get; set; }
    }
}
