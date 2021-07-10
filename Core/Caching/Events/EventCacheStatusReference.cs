using System;

namespace Zidium.Core.Caching
{
    public class EventCacheStatusReference
    {
        public Guid StatusId { get; set; }

        public bool Saved { get; set; }
    }
}
