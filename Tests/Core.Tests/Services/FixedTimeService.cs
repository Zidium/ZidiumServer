using System;
using Zidium.Core.Common;

namespace Zidium.Core.Tests.Services
{
    public class FixedTimeService : ITimeService
    {
        private DateTime _fixedTime = DateTime.Now;

        public DateTime Now()
        {
            return _fixedTime;
        }

        public void Add(TimeSpan time)
        {
            _fixedTime = _fixedTime.Add(time);
        }

        public void Set(DateTime time)
        {
            _fixedTime = time;
        }
    }
}
