using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models
{
    public class ComponentShowTimelinesGroupItemModel
    {
        public string Name { get; set; }

        public string Comment { get; set; }

        public string Action { get; set; }

        public Guid OwnerId { get; set; }

        public EventCategory? Category { get; set; }

        public EventImportance? Importance { get; set; }

        public Guid? EventTypeId { get; set; }

        public int? Count { get; set; }

        public string Url { get; set; }
    }
}