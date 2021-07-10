using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class ArchivedStatusGetForNotificationsInfo
    {
        public long Id;

        public EventInfo Event;

        public class EventInfo
        {
            public Guid Id;

            public Guid OwnerId;

            public EventCategory Category;

            public EventImportance Importance;

            public EventImportance PreviousImportance;

            public DateTime CreateDate;

            public DateTime StartDate;

            public DateTime ActualDate;
        }
    }
}
