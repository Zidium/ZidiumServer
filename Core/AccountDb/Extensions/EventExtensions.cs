using System;
using Zidium.Api.Dto;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public static class EventExtensions
    {
        /// <summary>
        /// Юнит-тесты могут НЕ передавать сообщение в результате, поэтому для таких результатов сгенерируем message сами
        /// </summary>
        /// <returns></returns>
        public static string GetUnitTestMessage(this EventForRead eventObj)
        {
            if (string.IsNullOrEmpty(eventObj.Message))
            {
                if (eventObj.Importance == EventImportance.Alarm)
                {
                    return "alarm";
                }
                if (eventObj.Importance == EventImportance.Warning)
                {
                    return "warning";
                }
                if (eventObj.Importance == EventImportance.Success)
                {
                    return "success";
                }
                if (eventObj.Importance == EventImportance.Unknown)
                {
                    return "unknown";
                }
            }
            return eventObj.Message;
        }

        public static DateTime GetEndDate(this EventForRead eventObj, DateTime now)
        {
            return eventObj.StartDate + EventHelper.GetDuration(eventObj.StartDate, eventObj.ActualDate, now);
        }

        /// <summary>
        /// Статус стал более важным (опасным). Используется для событий статусов.
        /// </summary>
        public static bool ImportanceHasGrown(this EventForRead eventObj)
        {
            return eventObj.Importance > eventObj.PreviousImportance;
        }

        public static TimeSpan GetDuration(this EventForRead eventObj, DateTime now)
        {
            return EventHelper.GetDuration(eventObj.StartDate, eventObj.ActualDate, now);
        }

        public static TimeSpan GetActualInterval(this EventForRead eventObj)
        {
            return eventObj.ActualDate - eventObj.EndDate;
        }

        public static long GetSize(this SendEventRequestDataDto data)
        {
            long size = 0;
            if (data.Message != null)
            {
                size += data.Message.Length * sizeof(char);
            }
            if (data.Version != null)
            {
                size += data.Version.Length * sizeof(char);
            }
            if (data.Properties != null)
            {
                foreach (var property in data.Properties)
                {
                    if (property != null)
                    {
                        size += property.GetSize();
                    }
                }
            }
            return size;
        }
    }
}
