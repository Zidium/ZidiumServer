using System;

namespace Zidium.UserAccount.Models.ComponentHistory
{
    public class ComponentsTreeItemModel
    {
        public Guid Id;

        public string DisplayName;

        public string SystemName;

        public string Path;

        public int OkTime;

        public Guid? ParentId;

        public Guid ComponentTypeId;

        public bool Expanded;

        public ComponentsTreeItemContentModel Content;

        public bool IsRoot
        {
            get { return ParentId == null; }
        }

        public DateTime From;

        public DateTime To;

        public TimelineModel Timeline;
    }
}