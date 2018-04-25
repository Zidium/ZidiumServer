using System;
using System.Collections.Generic;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Models.Metrics
{
    public class ShowModel
    {
        public Metric Metric { get; set; }

        public string ConditionRed { get; set; }

        public string ConditionYellow { get; set; }

        public string ConditionGreen { get; set; }

        public ObjectColor? ElseColor { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public TimeSpan? ActualInterval { get; set; }

        public const int LastValuesCountMax = 20;

        public List<MetricHistory> Values { get; set; }
    }
}