using System;
using Zidium.Core.Common;

namespace Zidium.Core.Api
{
    public class UpdateUnitTestRequestData
    {
        public Guid? UnitTestId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public Guid? ComponentId { get; set; }

        public double? PeriodSeconds { get; set; }

        public int? ActualTime { get; set; }

        public UnitTestResult? ErrorColor { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public bool? SimpleMode { get; set; }
    }
}
