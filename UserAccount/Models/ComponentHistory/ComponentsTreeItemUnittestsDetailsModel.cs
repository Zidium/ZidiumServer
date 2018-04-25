using System;

namespace Zidium.UserAccount.Models.ComponentHistory
{
    public class ComponentsTreeItemUnittestsDetailsModel
    {
        public ComponentsTreeItemUnittestsDetailsItemModel[] Items { get; set; }

        public bool Expanded { get; set; }

        public Guid Id { get; set; }

        public string HtmlId
        {
            get
            {
                return "unittests-" + Id;
            }
        }
    }
}