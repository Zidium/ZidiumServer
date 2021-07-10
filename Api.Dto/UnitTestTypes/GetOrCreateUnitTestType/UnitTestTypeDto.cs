using System;

namespace Zidium.Api.Dto
{
    public class UnitTestTypeDto
    {
        public Guid Id { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public bool IsSystem { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public int? ActualTimeSecs { get; set; }
    }
}
