using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class EventsListByStatusModel
    {
        public Event StatusEvent { get; set; }

        public Component Component { get; set; }

        public IQueryable<EventsListItemModel> Events { get; set; }

        public Dictionary<Guid, EventsListEventTypeModel> EventTypes { get; set; }
    }
}