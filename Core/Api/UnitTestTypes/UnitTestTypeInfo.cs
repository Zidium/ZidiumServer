using System;
using Zidium.Storage;

namespace Zidium.Core.Api
{
    public class UnitTestTypeInfo
    {
        public Guid Id { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public bool IsSystem { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public int? ActualTimeSecs { get; set; }
    }
}
