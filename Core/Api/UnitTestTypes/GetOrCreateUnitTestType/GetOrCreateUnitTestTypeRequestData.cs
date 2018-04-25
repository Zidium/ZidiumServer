using Zidium.Core.Common;

namespace Zidium.Core.Api
{
    public class GetOrCreateUnitTestTypeRequestData
    {
        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public ObjectColor? NoSignalColor { get; set; }

        public int? ActualTimeSecs { get; set; }
    }
}
