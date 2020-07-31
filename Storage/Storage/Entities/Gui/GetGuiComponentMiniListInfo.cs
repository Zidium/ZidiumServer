using System;

namespace Zidium.Storage
{
    public class GetGuiComponentMiniListInfo
    {
        public Guid Id { get; set; }

        public Guid ComponentTypeId { get; set; }

        public MonitoringStatus Status { get; set; }

        public string DisplayName { get; set; }

        public string SystemName { get; set; }
    }
}
