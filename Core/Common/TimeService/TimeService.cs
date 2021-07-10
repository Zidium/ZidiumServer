using System;

namespace Zidium.Core.Common.TimeService
{
    public class TimeService : ITimeService
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
