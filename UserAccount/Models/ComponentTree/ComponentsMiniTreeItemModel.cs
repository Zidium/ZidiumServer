using System;
using System.Collections.Generic;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class ComponentsMiniTreeItemModel
    {
        public Guid Id;

        public string DisplayName;

        public string SystemName;

        public MonitoringStatus Status;

        public bool IsRoot;

        public bool IsFolder;

        public Guid? ParentId;

        public ComponentsMiniTreeItemModel Parent;

        public List<ComponentsMiniTreeItemModel> Childs;

        public Guid ComponentTypeId;

        public string NameForSearch
        {
            get { return Id.ToString() + '~' + DisplayName + '~' + SystemName; }
        }

        public string FullName
        {
            get
            {
                var list = new List<string>();
                var component = this;
                while (component != null)
                {
                    list.Add(component.DisplayName);
                    component = component.Parent;
                }
                list.Reverse();
                return string.Join(" / ", list);
            }
        }

        public ComponentsMiniTreeItemModel()
        {
            Childs = new List<ComponentsMiniTreeItemModel>();
        }
    }
}