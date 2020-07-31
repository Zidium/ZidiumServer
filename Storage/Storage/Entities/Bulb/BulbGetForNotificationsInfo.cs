using System;
using System.Collections.Generic;

namespace Zidium.Storage
{
    public class BulbGetForNotificationsInfo
    {
        public Guid Id;

        public Guid ComponentId;

        public Guid ComponentTypeId;

        public List<ComponentGetForNotificationsInfo.LastComponentNotificationInfo> LastComponentNotifications;

        public EventCategory Category;

        public Guid StatusEventId;

        public MonitoringStatus Status;

        public MonitoringStatus PreviousStatus;

        public DateTime StartDate;

        public DateTime ActualDate;

    }
}
