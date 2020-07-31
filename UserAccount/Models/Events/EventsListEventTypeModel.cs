using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class EventsListEventTypeModel
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string SystemName { get; set; }

        public string Code { get; set; }

        public EventCategory Category { get; set; }
    }
}