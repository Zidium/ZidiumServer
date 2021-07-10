using System;

namespace Zidium.UserAccount.Models
{
    public class EventsListComponentModel
    {
        public Guid Id { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public string FullName { get; set; }
    }
}