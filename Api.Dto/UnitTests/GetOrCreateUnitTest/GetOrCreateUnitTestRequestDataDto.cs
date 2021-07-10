using System;

namespace Zidium.Api.Dto
{
    public class GetOrCreateUnitTestRequestDataDto
    {
        public Guid? NewId { get; set; }

        public Guid? ComponentId { get; set; }

        public Guid? UnitTestTypeId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public UnitTestResult? ErrorColor { get; set; }

        public double? ActualTimeSecs { get; set; }

        public int? AttempMax { get; set; }

        public int? PeriodSeconds { get; set; }

        public bool? SimpleMode { get; set; }
    }
}
