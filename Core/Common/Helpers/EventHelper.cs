﻿using System;
using Zidium.Core.Caching;
using Zidium.Storage;

namespace Zidium.Core.Common.Helpers
{
    public static class EventHelper
    {
        /// <summary>
        /// Проверяет можно ли склеивать события.
        /// Все события склеиваются одинаково (простые и непрерывные).
        /// </summary>
        /// <param name="oldEvent"></param>
        /// <param name="newEvent"></param>
        /// <param name="maxJoinDate"></param>
        /// <returns></returns>
        private static bool CanJoin(
            IEventCacheReadObject oldEvent,
            EventForAdd newEvent,
            DateTime maxJoinDate)
        {
            // склеивать события разных версий нельзя
            if (newEvent.VersionLong != null
                && oldEvent.VersionLong != null // если хотя бы у одного события версия неизвестна, то можно клеить
                && newEvent.VersionLong != oldEvent.VersionLong)
            {
                return false;
            }

            return oldEvent.OwnerId == newEvent.OwnerId
                   && oldEvent.Importance == newEvent.Importance
                   && oldEvent.IsSpace == newEvent.IsSpace // склеивать пробел с сигналом нельзя
                   && oldEvent.EventTypeId == newEvent.EventTypeId
                   && oldEvent.JoinKeyHash == newEvent.JoinKeyHash
                   && maxJoinDate >= newEvent.StartDate;
        }

        public static bool CanJoinSimpleEvents(IEventCacheReadObject oldEvent, EventForAdd newEvent, TimeSpan joinInterval)
        {
            if (oldEvent == null)
            {
                return false;
            }
            if (joinInterval == TimeSpan.Zero)
            {
                return false;
            }
            // добавляем 1 секунду, т.к. при сохранении в БД мс теряются
            var maxJoinDate = oldEvent.EndDate.AddSeconds(1) + joinInterval;

            return CanJoin(oldEvent, newEvent, maxJoinDate);
        }

        public static TimeSpan GetDuration(DateTime startDate, DateTime actualDate, DateTime now)
        {
            if (actualDate > now)
                actualDate = now;

            var duration = actualDate - startDate;

            if (duration < TimeSpan.Zero)
                duration = TimeSpan.Zero;

            return duration;
        }
    }
}
