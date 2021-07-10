using System;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class ComponentsTreeModel
    {
        public Guid? ComponentTypeId { get; set; }

        public ColorStatusSelectorValue Color { get; set; }

        public string Search { get; set; }

        public const string ExpandedItemsCookieName = "ComponentTreeExpandedItems";
    }
}