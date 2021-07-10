using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class GetGuiComponentMiniTreeInfo
    {
        public Guid Id;

        public string DisplayName;

        public string SystemName;

        public MonitoringStatus Status;

        public Guid? ParentId;

        public Guid ComponentTypeId;
    }
}
