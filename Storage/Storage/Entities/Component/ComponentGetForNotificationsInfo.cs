using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class ComponentGetForNotificationsInfo
    {
        public Guid Id;

        public Guid ComponentTypeId;

        public List<LastComponentNotificationInfo> LastComponentNotifications;

        public class LastComponentNotificationInfo
        {
            public Guid Id;

            public string Address;

            public SubscriptionChannel Type;

            public EventImportance EventImportance;

            public DateTime CreateDate;

            public Guid EventId;

        }
    }
}
