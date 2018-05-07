using System;

namespace Zidium.Core.Api
{
    public class AddCheckBaseResponseData
    {
        public Guid Id { get; set; }

        public Guid ComponentId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public int PeriodSeconds { get; set; }

        public UnitTestResult? ErrorColor { get; set; }

        public int AttempMax { get; set; }
    }
}
