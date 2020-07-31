using System;
using Zidium.Storage;

namespace Zidium.Core.Api
{
    public class GetOrCreateUnitTestRequestData
    {
        public Guid? ComponentId { get; set; }

        public Guid? UnitTestTypeId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public int? PeriodSeconds { get; set; }

        public UnitTestResult? ErrorColor { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public int? ActualTimeSecs { get; set; }

        public bool? SimpleMode { get; set; }

        public int? AttempMax { get; set; }

        /// <summary>
        /// Если ИД указан, то при создании проверки будет использоваться данный ИД
        /// Используется при содании "простых" проверок
        /// </summary>
        public Guid? NewId { get; set; }
    }
}
