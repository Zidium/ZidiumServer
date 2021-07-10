using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public static class BulbExtensions
    {
        public static string GetUnitTestMessage(this BulbForRead bulb)
        {
            if (string.IsNullOrEmpty(bulb.Message))
            {
                if (bulb.Status == MonitoringStatus.Alarm)
                {
                    return "alarm";
                }
                if (bulb.Status == MonitoringStatus.Warning)
                {
                    return "warning";
                }
                if (bulb.Status == MonitoringStatus.Success)
                {
                    return "success";
                }
                if (bulb.Status == MonitoringStatus.Unknown)
                {
                    return "unknown";
                }
                if (bulb.Status == MonitoringStatus.Disabled)
                {
                    return "disabled";
                }
            }
            return bulb.Message;
        }

        public static TimeSpan GetDuration(DateTime startDate, DateTime actualDate, DateTime now)
        {
            if (actualDate > now)
            {
                return now - startDate;
            }
            return actualDate - startDate;
        }

        public static TimeSpan GetDuration(this BulbForRead bulb, DateTime now)
        {
            return GetDuration(bulb.StartDate, bulb.ActualDate, now);
        }

        public static bool IsActualOnDate(this BulbForRead bulb, DateTime date)
        {
            return bulb.ActualDate >= date;
        }

        public static Guid GetOwnerId(this BulbForRead bulb)
        {
            if (bulb.ComponentId.HasValue)
            {
                return bulb.ComponentId.Value;
            }
            if (bulb.UnitTestId.HasValue)
            {
                return bulb.UnitTestId.Value;
            }
            if (bulb.MetricId.HasValue)
            {
                return bulb.MetricId.Value;
            }
            throw new Exception("Не удалось найти OwnerId");
        }

        /// <summary>
        /// Лист дерева статусов (НЕ имеет детей)
        /// </summary>
        public static bool IsLeaf(this BulbForRead bulb)
        {
            return bulb.EventCategory == EventCategory.MetricStatus
                   || bulb.EventCategory == EventCategory.UnitTestStatus
                   || bulb.EventCategory == EventCategory.ComponentEventsStatus;
        }
    }
}
