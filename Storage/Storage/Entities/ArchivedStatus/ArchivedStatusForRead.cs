using System;

namespace Zidium.Storage
{
    public class ArchivedStatusForRead
    {
        public ArchivedStatusForRead(long id, Guid eventId)
        {
            Id = id;
            EventId = eventId;
        }

        public long Id { get; }

        public Guid EventId { get; }

    }
}
