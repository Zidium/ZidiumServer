using System;

namespace Zidium.Storage.Ef
{
    public class DbEventStatus
    {
        public Guid EventId { get; set; }

        public virtual DbEvent Event { get; set; }

        public Guid StatusId { get; set; }

        public virtual DbEvent StatusEvent { get; set; }
    }
}
