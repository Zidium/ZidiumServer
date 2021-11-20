using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public interface IEventRepository
    {
        void Add(EventForAdd entity);

        void Add(EventForAdd[] entities);

        void Update(EventForUpdate entity);

        void Update(EventForUpdate[] entities);

        EventForRead GetOneById(Guid id);

        EventForRead GetOneOrNullById(Guid id);

        EventForRead[] GetMany(Guid[] ids);

        EventForRead GetLastEventByEndDate(Guid eventTypeId);

        EventForRead GetForJoin(EventForAdd eventObj);

        EventForRead[] Filter(
            Guid ownerId,
            EventCategory? category,
            DateTime? from,
            DateTime? to,
            EventImportance[] importance,
            Guid? eventTypeId,
            string searthText,
            int maxCount);

        EventForRead GetMostDangerousEvent(
            Guid componentId,
            EventCategory[] categories,
            DateTime fromDate
            );

        EventForRead[] GetActualByType(Guid eventTypeId, DateTime actualDate);

        int DeleteEventParameters(EventCategory[] categories, int maxCount, DateTime toDate);

        int DeleteEvents(EventCategory[] categories, int maxCount, DateTime toDate);

        int DeleteEventStatuses(EventCategory[] categories, int maxCount, DateTime toDate);

        int UpdateMetricsHistory(EventCategory[] categories, int maxCount, DateTime toDate);

        int DeleteNotifications(EventCategory[] categories, int maxCount, DateTime toDate);

        int DeleteEventArchivedStatuses(EventCategory[] categories, int maxCount, DateTime toDate);

        int GetEventsCountForDeletion(EventCategory[] categories, DateTime toDate);

        EventForRead GetFirstEventReason(Guid eventId);

        EventForRead GetRecentEventReason(Guid eventId);

        EventForRead[] GetEventReasons(Guid eventId);

        /*
        EventForRead[] GetActualEventsWithWrongData(DateTime actualDate);

        EventForRead[] GetEventsWithWrongData(EventForRead actualEvent, DateTime actualDate);
        */

        EventForRead[] Filter(Guid ownerId, EventCategory category, DateTime from, DateTime to);

        EventForRead[] Filter(Guid ownerId, EventCategory[] categories, DateTime from, DateTime to);

        EventForRead[] Filter(Guid ownerId, Guid eventTypeId, DateTime from, DateTime to);

        EventForRead[] Filter(Guid[] ownerIds, Guid eventTypeId, DateTime from, DateTime to);

        EventForRead[] Filter(Guid eventTypeId, DateTime from, DateTime to);

        int GetErrorsCountByPeriod(Guid componentId, DateTime from, DateTime to);

        EventForRead[] GetErrorsByPeriod(Guid? componentId, DateTime from, DateTime to);

        EventForRead[] GetErrorsByPeriod(Guid[] componentIds, DateTime from, DateTime to);

        EventForRead GetLastEvent(Guid ownerId, EventCategory category);

        EventForRead[] GetLastEvents(Guid ownerId, EventCategory category, int maxCount);

        EventForRead[] GetByEventTypeId(Guid eventTypeId);

        EventForRead[] Filter(
            Guid? ownerId,
            Guid? componentTypeId,
            Guid? eventTypeId,
            EventCategory[] categories,
            EventImportance[] importances,
            DateTime? fromDate,
            DateTime? toDate,
            string message,
            string versionFrom,
            int maxCount
            );

        EventForRead GetByOwnerIdAndLastStatusEventId(Guid ownerId, Guid lastStatusEventId);

        void AddStatusToEvent(EventStatusForAdd[] statuses);

    }
}
