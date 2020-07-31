using System;

namespace Zidium.Storage.Ef
{
    public class DbArchivedStatus
    {
        public long Id { get; set; }

        public Guid EventId { get; set; }

        public virtual DbEvent Event { get; set; }
    }
}
