using System;
using Zidium.Core.Api;
using Zidium.Core.Common;

namespace Zidium.Core
{
    public static class ImportanceHelper
    {
        public static EventImportance Get(MonitoringStatus status)
        {
            if (status == MonitoringStatus.Alarm)
            {
                return EventImportance.Alarm;
            }
            if (status == MonitoringStatus.Warning)
            {
                return EventImportance.Warning;
            }
            if (status == MonitoringStatus.Success)
            {
                return EventImportance.Success;
            }
            if (status == MonitoringStatus.Unknown)
            {
                return EventImportance.Unknown;
            }
            if (status == MonitoringStatus.Disabled)
            {
                return EventImportance.Unknown;
            }
            throw new Exception("Неизвестное значение UnitTestResultStatus: " + status);
        }

        public static EventImportance Get(ObjectColor color)
        {
            if (color == ObjectColor.Red)
            {
                return EventImportance.Alarm;
            }
            if (color == ObjectColor.Yellow)
            {
                return EventImportance.Warning;
            }
            if (color == ObjectColor.Green)
            {
                return EventImportance.Success;
            }
            if (color == ObjectColor.Gray)
            {
                return EventImportance.Unknown;
            }
            throw new Exception("Неизвестное значение ObjectColor: " + color);
        }

        public static EventImportance? Get(ObjectColor? color)
        {
            if (color == null)
                return null;
            return Get(color.Value);
        }

        public static string GetSystemName(EventImportance importance)
        {
            if (importance == EventImportance.Alarm)
            {
                return "alarm";
            }
            if (importance == EventImportance.Warning)
            {
                return "warning";
            }
            if (importance == EventImportance.Success)
            {
                return "success";
            }
            if (importance == EventImportance.Unknown)
            {
                return "unknown";
            }
            throw new Exception("Неизвестное значение EventImportance: " + importance);
        }
    }
}
