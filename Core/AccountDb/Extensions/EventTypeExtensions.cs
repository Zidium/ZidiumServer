using System;
using Zidium.Storage;

namespace Zidium.Core.AccountDb
{
    public static class EventTypeExtensions
    {
        public static TimeSpan? JoinInterval(this EventTypeForRead eventType)
        {
            if (eventType.JoinIntervalSeconds == null)
                return null;
            return TimeSpan.FromSeconds(eventType.JoinIntervalSeconds.Value);
        }
    }
}
