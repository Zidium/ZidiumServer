using System;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class UpdateUnitTestTypeRequestData
    {
        public Guid? UnitTestTypeId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public int? ActualTimeSecs { get; set; }
    }
}
