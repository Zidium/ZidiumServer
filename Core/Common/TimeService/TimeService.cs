using System;

namespace Zidium.Core.Common
{
    public class TimeService : ITimeService
    {
        public DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }
}
