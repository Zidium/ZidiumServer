using System;
using Zidium.Storage;

namespace Zidium.Core
{
    public static class EventImportanceHelper
    {
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
            throw new Exception("Неизвестное значение ComponentStatus: " + status);
        }

        public static EventImportance Get(UnitTestResult result)
        {
            if (result == UnitTestResult.Alarm)
            {
                return EventImportance.Alarm;
            }
            if (result == UnitTestResult.Warning)
            {
                return EventImportance.Warning;
            }
            if (result == UnitTestResult.Success)
            {
                return EventImportance.Success;
            }
            if (result == UnitTestResult.Unknown)
            {
                return EventImportance.Unknown;
            }
            throw new Exception("Неизвестное значение UnitTestResult: " + result);
        }
    }
}
