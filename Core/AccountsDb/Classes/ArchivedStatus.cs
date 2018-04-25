using System;

namespace Zidium.Core.AccountsDb
{
    public class ArchivedStatus
    {
        public long Id { get; set; }

        public Guid EventId { get; set; }

        public virtual Event Event { get; set; }
    }
}
