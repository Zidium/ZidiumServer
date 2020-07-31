using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class SimplifiedStatusData
    {
        public MonitoringStatus Status;

        public DateTime ActualDate;

        public DateTime StartDate;

        public TimeSpan GetDuration(DateTime now)
        {
            if (ActualDate > now)
            {
                return now - StartDate;
            }
            return ActualDate - StartDate;
        }

        public Guid? FirstEventId;

        public string Message;
    }
}