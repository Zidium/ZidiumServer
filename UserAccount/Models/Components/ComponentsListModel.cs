using System;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class ComponentsListModel
    {
        public Guid? ComponentTypeId { get; set; }

        public ColorStatusSelectorValue Color { get; set; }

        public Guid? ParentComponentId { get; set; }

        public bool ShowDeleted { get; set; }

        public string Search { get; set; }

        public IQueryable<Component> Components { get; set; }

        public static int MaxMessageLength = 255;

        public string GetComponentStatusTextCssClass(Component component)
        {
            var status = component.ExternalStatus.Status;
            var result = GuiHelper.GetComponentStatusTextCssClass(status);
            return result;
        }
    }
}