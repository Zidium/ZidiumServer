using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models
{
    public class ComponentsMiniListItemModel
    {
        public Guid Id { get; set; }

        public Guid ComponentTypeId { get; set; }

        public MonitoringStatus Status { get; set; }

        public string DisplayName { get; set; }

        public string SystemName { get; set; }

        public string NameForSearch
        {
            get { return Id.ToString() + '~' + DisplayName + '~' + SystemName; }
        }
    }
}