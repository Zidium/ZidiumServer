using System;
using Zidium.Core.AccountDb;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Storage;
using Zidium.Api.Dto;

namespace Zidium.Core.AccountsDb
{
    public interface IEventService
    {
        /// <summary>
        /// Отправляет или склеивает событие
        /// </summary>
        IEventCacheReadObject SendEvent(Guid componentId, SendEventRequestDataDto eventData);

        IEventCacheReadObject JoinEvent(Guid componentId, JoinEventRequestDataDto joinEventData);

        /// <summary>
        /// Возвращает самое опасное событие
        /// </summary>
        EventForRead GetDangerousEvent(Guid componentId,
            DateTime fromDate);

        IEventCacheReadObject GetEventCacheOrNullById(Guid eventId);

        EventForRead[] GetEvents(GetEventsRequestDataDto filter);

        void Update(EventForUpdate eventObj);

        // TODO Remove trivial method
        void Add(EventForAdd eventObj);

        EventForRead GetRecentReasonEvent(EventForRead statusEvent);

        /// <summary>
        /// Получает элементы для диаграммы состояний по категории
        /// </summary>
        TimelineState[] GetTimelineStates(Guid ownerId, EventCategory category, DateTime from, DateTime to);

        /// <summary>
        /// Получает элементы для диаграммы состояний по типу события для одного компонента
        /// </summary>
        TimelineState[] GetTimelineStates(Guid ownerId, Guid eventTypeId, DateTime from, DateTime to);

        /// <summary>
        /// Получает элементы для диаграммы состояний по типу события независимо от компонента
        /// </summary>
        TimelineState[] GetTimelineStatesAnyComponents(Guid eventTypeId, DateTime @from, DateTime to);

        /// <summary>
        /// Рассчитывает OkTime по элементам диаграммы состояний 
        /// </summary>
        int GetTimelineOkTime(TimelineState[] states, DateTime from, DateTime to);

    }
}
