using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class GetGuiChecksResultsInfo
    {
        public Guid Id;

        public BulbInfo Bulb;

        public TypeInfo Type;

        public ComponentInfo Component;

        public class BulbInfo
        {
            public MonitoringStatus Status;
        }

        public class TypeInfo
        {
            public Guid Id;

            public bool IsSystem;

            public string DisplayName;
        }

        public class ComponentInfo
        {
            public Guid Id;

            public ComponentTypeInfo ComponentType;
        }

        public class ComponentTypeInfo
        {
            public Guid Id;

            public string DisplayName;
        }
    }
}
