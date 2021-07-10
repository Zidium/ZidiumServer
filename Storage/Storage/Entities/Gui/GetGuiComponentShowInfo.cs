using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class GetGuiComponentShowInfo
    {
        public ActualEventMiniInfo[] ActualEventsMiniInfo;

        public UnitTestMiniInfo[] UnitTestsMiniInfo;

        public MetricMiniInfo[] MetricsMiniInfo;

        public ChildMiniInfo[] ChildsMiniInfo;

        public UnitTestInfo[] UnitTests;

        public MetricInfo[] Metrics;

        public ChildInfo[] Childs;

        public class ActualEventMiniInfo
        {
            public Guid Id;

            public EventImportance Importance;
        }

        public class UnitTestMiniInfo
        {
            public Guid Id;

            public MonitoringStatus Status;
        }

        public class MetricMiniInfo
        {
            public Guid Id;

            public MonitoringStatus Status;
        }

        public class ChildMiniInfo
        {
            public Guid Id;

            public MonitoringStatus Status;
        }

        public class UnitTestInfo
        {
            public Guid Id;

            public string DisplayName;

            public BulbForRead Bulb;

            public UnitTestTypeInfo Type;

            public int? PeriodSeconds;

            public bool Enable;
        }

        public class MetricInfo
        {
            public Guid Id;

            public string DisplayName;

            public BulbForRead Bulb;

            public MetricTypeInfo MetricType;

            public double? Value;

            public bool Enable;
        }

        public class ChildInfo
        {
            public Guid Id;

            public string DisplayName;

            public BulbForRead ExternalStatus;

            public ComponentTypeInfo ComponentType;

            public bool Enable;
        }

        public class UnitTestTypeInfo
        {
            public Guid Id;

            public string DisplayName;

            public bool IsSystem;
        }

        public class MetricTypeInfo
        {
            public Guid Id;

            public string DisplayName;
        }

        public class ComponentTypeInfo
        {
            public Guid Id;

            public string DisplayName;
        }
    }
}
