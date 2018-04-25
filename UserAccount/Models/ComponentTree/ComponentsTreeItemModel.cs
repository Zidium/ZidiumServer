using System;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class ComponentsTreeItemModel
    {
        public Guid Id;

        public string DisplayName;

        public string SystemName;

        public MonitoringStatus Status;

        public TimeSpan StatusDuration;

        public bool IsRoot;

        public bool IsFolder;

        public Guid? ParentId;

        public Guid ComponentTypeId;

        public bool Expanded;

        public ComponentsTreeItemContentModel Content;
    }
}