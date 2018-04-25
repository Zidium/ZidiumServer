using System;
using Zidium.Core.Api;

namespace Zidium.Core.Common.Helpers
{
    public static class MonitoringStatusHelper
    {
        public static MonitoringStatus Get(EventImportance eventImportance)
        {
            if (eventImportance == EventImportance.Alarm)
            {
                return MonitoringStatus.Alarm;
            }
            if (eventImportance == EventImportance.Warning)
            {
                return MonitoringStatus.Warning;
            }
            if (eventImportance == EventImportance.Success)
            {
                return MonitoringStatus.Success;
            }
            if (eventImportance == EventImportance.Unknown)
            {
                return MonitoringStatus.Unknown;
            }
            throw new Exception("неизвестное значение EventImportance: " + eventImportance);
        }

        public static MonitoringStatus Get(ObjectColor color)
        {
            if (color == ObjectColor.Gray)
            {
                return MonitoringStatus.Unknown;
            }
            if (color == ObjectColor.Green)
            {
                return MonitoringStatus.Success;
            }
            if (color == ObjectColor.Yellow)
            {
                return MonitoringStatus.Warning;
            }
            if (color == ObjectColor.Red)
            {
                return MonitoringStatus.Alarm;
            }
            throw new Exception("Неизвестный ObjectColor " + color);
        }
    }
}
