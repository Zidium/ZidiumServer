using System;
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
    }
}
