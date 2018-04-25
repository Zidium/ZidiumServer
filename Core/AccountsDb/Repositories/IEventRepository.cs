using System;
using System.Linq;
using System.Collections.Generic;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public interface IEventRepository
    {
        IQueryable<Event> GetErrorsByPeriod(DateTime @from, DateTime to);

        /// <summary>
        /// Возвращает события, по которым нужно создать уведомления
        /// Только те, по которым ещё не было уведомлений
        /// </summary>
        /// <returns></returns>
        IQueryable<Event> GetArhiveForNotificationFirst(Guid? componentId, int maxCount);

        /// <summary>
        /// Возвращает события, по которым нужно создать уведомления
        /// Только те, которые поменялись с предыдущего уведомления
        /// </summary>
        /// <returns></returns>
        IQueryable<Event> GetArhiveForNotificationChanged(Guid? componentId, int maxCount);

        IQueryable<Event> GetActualForNotification(Guid? componentId);
        
        /// <summary>
        /// Получение события по ключу слияния
        /// </summary>
        /// <param name="joinKeyHash"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        Event GetByHashKey(long joinKeyHash, Guid componentId); //Используется в юнит-тестах

        IQueryable<Event> QueryAllByComponentId(Guid[] componentId);

        IQueryable<Event> QueryAll(Guid ownerId);

        IQueryable<Event> QueryAllByAccount();

        IQueryable<Event> GetAllByType(Guid eventTypeId);

        /// <summary>
        /// Получение событий компонента по критериям
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="category"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="importance"></param>
        /// <param name="eventTypeId"></param>
        /// <param name="searthText"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        IQueryable<Event> QueryAll(Guid ownerId, EventCategory? category, DateTime? @from, DateTime? to, List<EventImportance> importance, Guid? eventTypeId, string searthText, int maxCount);

        /// <summary>
        /// Получаем последнее событие для склейки
        /// </summary>
        /// <returns></returns>
        Event GetForJoin(Event newEvent);
        
        /// <summary>
        /// Возвращает список актуальных клиентских событий
        /// Клиентских = значит сюда не входят события статусов, проверок и т.д.
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        IQueryable<Event> GetActualClientEvents(Guid componentId, DateTime now);

        Event Add(Event eventObj);

        Event GetById(Guid eventId);

        Event GetByIdOrNull(Guid eventId);

        IQueryable<Event> GetEventReasons(Guid eventId);

        Event GetLastEventByEndDate(Guid eventTypeId);

        int DeleteEventParameters(string categories, int maxCount, DateTime toDate);

        int DeleteEvents(string categories, int maxCount, DateTime toDate);

        int DeleteEventStatuses(string categories, int maxCount, DateTime toDate);

        int UpdateMetricsHistory(string categories, int maxCount, DateTime toDate);

        int DeleteNotifications(string categories, int maxCount, DateTime toDate);

        int DeleteEventArchivedStatuses(string categories, int maxCount, DateTime toDate);

        int GetEventsCountForDeletion(string categories, DateTime toDate);

        string DebugGetEventStatusesData(string categories, int maxCount, DateTime toDate);

        /// <summary>
        /// Получает последнюю причину для статусного события
        /// </summary>
        Event GetRecentReasonEvent(Event statusEvent);

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
