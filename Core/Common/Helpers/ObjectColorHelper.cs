using System;
using Zidium.Api.Dto;

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

        public static ObjectColor Get(UnitTestResult result)
        {
            if (result == UnitTestResult.Alarm)
            {
                return ObjectColor.Red;
            }
            if (result == UnitTestResult.Warning)
            {
                return ObjectColor.Yellow;
            }
            if (result == UnitTestResult.Success)
            {
                return ObjectColor.Green;
            }
            if (result == UnitTestResult.Unknown)
            {
                return ObjectColor.Gray;
            }
            throw new Exception("Неизвестный UnitTestResult " + result);
        }
    }
}
