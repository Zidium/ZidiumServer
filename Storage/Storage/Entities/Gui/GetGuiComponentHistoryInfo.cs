using System;
using System.Collections.Generic;

namespace Zidium.Storage
{
    public class GetGuiComponentHistoryInfo
    {
        public Guid Id;

        public string DisplayName;

        public string SystemName;

        public Guid? ParentId;

        public Guid ComponentTypeId;

        public bool HasEvents;

        public List<UnitTestInfo> UnitTests;

        public List<MetricInfo> Metrics;

        public class UnitTestInfo
        {
            public Guid Id;

            public string DisplayName;
        }

        public class MetricInfo
        {
            public Guid Id;

            public string DisplayName;
        }
    }
}
