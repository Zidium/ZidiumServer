using System;

namespace Zidium.Storage
{
    public class GetGuiComponentListInfo
    {
        public Guid Id;

        public string DisplayName;

        public string SystemName;

        public MonitoringStatus ExternalStatus;

        public ComponentTypeInfo ComponentType;

        public string Version;

        public class ComponentTypeInfo
        {
            public Guid Id;

            public string DisplayName;
        }
    }
}
