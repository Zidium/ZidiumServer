using System;
using System.Collections.Generic;
using Zidium.Core.Api;
using Zidium.Core.Caching;

namespace Zidium.Core.AccountsDb
{
    public interface IEventService
    {
        /// <summary>
        /// Отправляет или склеивает событие
        /// </summary>
        /// <returns></returns>
        IEventCacheReadObject SendEvent(Guid accountId, Guid componentId, SendEventData eventData);

        IEventCacheReadObject JoinEvent(Guid accountId, Guid componentId, JoinEventData joinEventData);

        /// <summary>
        /// Возвращает самое опасное событие
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="componentId"></param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        Event GetDangerousEvent(
            Guid accountId,
            Guid componentId, 
            DateTime fromDate);

        IEventCacheReadObject GetEventCacheOrNullById(Guid accountId, Guid eventId);

        Event GetEventOrNullById(Guid accountId, Guid eventId);

        Event GetEventById(Guid accountId, Guid eventId);

        List<Event> GetEvents(Guid accountId, GetEventsRequestData filter);

        Event Update(Guid accountId, Event _event);

        void Add(Guid accountId, Event eventObj);
    }
}
