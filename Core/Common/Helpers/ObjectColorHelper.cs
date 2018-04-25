using System;
using Zidium.Core.Api;

namespace Zidium.Core.Common.Helpers
{
    public static class ObjectColorHelper
    {
        public static ObjectColor Get(MonitoringStatus status)
        {
            if (status == MonitoringStatus.Alarm)
            {
                return ObjectColor.Red;
            }
            if (status == MonitoringStatus.Warning)
            {
                return ObjectColor.Yellow;
            }
            if (status == MonitoringStatus.Success)
            {
                return ObjectColor.Green;
            }
            if (status == MonitoringStatus.Unknown)
            {
                return ObjectColor.Gray;
            }
            if (status == MonitoringStatus.Disabled)
            {
                return ObjectColor.Gray;
            }
            throw new Exception("Неизвестный MonitoringStatus " + status);
        }
    }
}
