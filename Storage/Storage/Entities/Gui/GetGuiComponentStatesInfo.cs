using System;
using System.Collections.Generic;

namespace Zidium.Storage
{
    public class GetGuiComponentStatesInfo
    {
        public Guid Id;

        public string SystemName;

        public string DisplayName;

        public DateTime CreatedDate;

        public ComponentTypeInfo ComponentType;

        public List<EventInfo> Events;

        public List<UnitTestInfo> UnitTests;

        public List<ChildInfo> Childs;

        public List<MetricInfo> Metrics;

        public class ComponentTypeInfo
        {
            public Guid Id;

            public string DisplayName;
        }

        public class EventInfo
        {
            public Guid Id;

            public EventImportance Importance;
        }

        public class UnitTestInfo
        {
            public Guid Id;

            public MonitoringStatus Status;
        }

        public class ChildInfo
        {
            public Guid Id;

            public MonitoringStatus Status;
        }

        public class MetricInfo
        {
            public Guid Id;

            public MonitoringStatus Status;
        }
    }
}
