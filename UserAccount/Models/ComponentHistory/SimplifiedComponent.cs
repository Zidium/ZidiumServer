using System;
using System.Collections.Generic;

namespace Zidium.UserAccount.Models.ComponentHistory
{
    public class SimplifiedComponent
    {
        public Guid Id;

        public string DisplayName;

        public string SystemName;

        public Guid? ParentId;

        public SimplifiedComponent Parent;

        public Guid ComponentTypeId;

        public List<SimplifiedComponent> Childs;

        public bool HasEvents;

        public SimplifiedUnittest[] Unittests;

        public SimplifiedMetric[] Metrics;

        public SimplifiedComponent()
        {
            Childs = new List<SimplifiedComponent>();
        }

        public string Path
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
    }
}