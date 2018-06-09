using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.AccountsDb
{
    public class EventRepository : IEventRepository
    {
        protected AccountDbContext Context { get; set; }

        public EventRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public IQueryable<Event> GetArhiveForNotificationFirst(Guid? componentId, int maxCount)
        {
            var categories = new[] { EventCategory.ComponentExternalStatus, EventCategory.ComponentInternalStatus };

            var subQuery = Context
                .Events
                .Where(x =>
                    categories.Contains(x.Category) &&
                    x.LastNotificationDate == null &&
                    x.ActualDate < DateTimeHelper.InfiniteActualDate &&
                    x.StartDate != x.EndDate);

            if (componentId.HasValue)
                subQuery = subQuery.Where(t => t.OwnerId == componentId.Value);

            var subQuery2 = subQuery.OrderBy(t => t.StartDate).Select(t => t.Id).Take(maxCount);

            var result = Context.Events.Where(t => subQuery2.Contains(t.Id));

            return result;
        }

        public IQueryable<Event> GetArhiveForNotificationChanged(Guid? componentId, int maxCount)
        {
            var categories = new[] { EventCategory.ComponentExternalStatus, EventCategory.ComponentInternalStatus };

            var subQuery = Context.Events
                .Where(x =>
                    categories.Contains(x.Category) &&
                    x.LastUpdateDate > x.LastNotificationDate &&
                    x.ActualDate < DateTimeHelper.InfiniteActualDate &&
                    x.StartDate != x.EndDate);

            if (componentId.HasValue)
                subQuery = subQuery.Where(t => t.OwnerId == componentId.Value);

            var subQuery2 = subQuery.OrderBy(t => t.StartDate).Select(t => t.Id).Take(maxCount);

            var result = Context.Events.Where(t => subQuery2.Contains(t.Id));

            return result;
        }

        public IQueryable<Event> GetActualForNotification(Guid? componentId)
        {
            var categories = new[] { EventCategory.ComponentExternalStatus, EventCategory.ComponentInternalStatus };

            var subQuery = Context.Events
                .Where(x =>
                    categories.Contains(x.Category) &&
                    x.ActualDate == DateTimeHelper.InfiniteActualDate);

            if (componentId.HasValue)
                subQuery = subQuery.Where(t => t.OwnerId == componentId.Value);

            var subQuery2 = subQuery.OrderBy(t => t.StartDate).Select(t => t.Id);

            var result = Context.Events.Where(t => subQuery2.Contains(t.Id));

            return result;
        }

        public Event Add(Event eventObj)
        {
            if (eventObj.Category == 0)
            {
                throw new Exception("При сохранении события не указали категорию");
            }
            if (eventObj.Message != null && eventObj.Message.Length > 8000)
            {
                eventObj.Message = eventObj.Message.Substring(0, 8000);
            }
            if (eventObj.LastUpdateDate.Year < 2000)
            {
                eventObj.LastUpdateDate = DateTime.Now;
            }

            Context.Events.Add(eventObj);

            return eventObj;
        }

        public Event GetByHashKey(long joinKeyHash, Guid componentId)
        {
            return Context.Events.SingleOrDefault(t => t.OwnerId == componentId && t.JoinKeyHash == joinKeyHash);
        }

        public IQueryable<Event> QueryAll(Guid ownerId)
        {
            return Context.Events.Where(t => t.OwnerId == ownerId);
        }

        public IQueryable<Event> QueryAllByAccount()
        {
            return Context.Events;
        }

        public IQueryable<Event> QueryAllByComponentId(Guid[] componentId)
        {
            var query = Context.Events.Where(t => componentId.Contains(t.OwnerId));
            return query;
        }

        public IQueryable<Event> QueryAll(Guid ownerId, EventCategory? category, DateTime? @from, DateTime? to, List<EventImportance> importance, Guid? eventTypeId, string searthText, int maxCount)
        {
            var query = Context.Events.Where(t => t.OwnerId == ownerId);

            if (from.HasValue)
                query = query.Where(t => t.ActualDate >= from.Value);

            if (to.HasValue)
                query = query.Where(t => t.StartDate < to.Value);

            if (category.HasValue)
            {
                query = query.Where(t => t.Category == category);
            }

            if (importance != null && importance.Count > 0)
                query = query.Where(t => importance.Contains(t.Importance));

            if (eventTypeId.HasValue)
                query = query.Where(t => t.EventTypeId == eventTypeId.Value);

            if (string.IsNullOrEmpty(searthText) == false)
                query = query.Where(t => t.Message.Contains(searthText));

            query = query.OrderByDescending(t => t.StartDate).Take(maxCount);

            return query;
        }

        public Event GetForJoin(Event newEvent)
        {
            if (newEvent == null)
            {
                throw new ArgumentNullException("newEvent");
            }

            // Первичное условие - по владельцу и типу
            // Эти поля входят в индекс
            var query = Context.Events.Where(x =>
                x.OwnerId == newEvent.OwnerId
                && x.EventTypeId == newEvent.EventTypeId
                && x.Importance == newEvent.Importance
                );

            // Вторичные условия - по остальным параметрам, у них низкая селективность
            query = query.Where(x =>
                x.Category == newEvent.Category
                && x.JoinKeyHash == newEvent.JoinKeyHash
                && x.IsSpace == newEvent.IsSpace
                );

            // Версию сравниваем именно по строке
            // VersionLong заполнено только для версий формата x.x.x.x!
            if (newEvent.Version != null)
            {
                query = query.Where(x => x.Version == newEvent.Version);
            }

            var eventId = query.OrderByDescending(x => x.ActualDate).Select(t => t.Id).FirstOrDefault();

            return eventId != Guid.Empty ? Context.Events.Find(eventId) : null;
        }

        public Event GetById(Guid id)
        {
            var result = Context.Events.Find(id);

            if (result == null)
                throw new ObjectNotFoundException(id, Naming.Event);

            return result;
        }

        public Event GetByIdOrNull(Guid id)
        {
            return Context.Events.Find(id);
        }

        public IQueryable<Event> GetEventReasons(Guid eventId)
        {
            return QueryAllByAccount().Where(x => x.StatusEvents.Any(y => y.Id == eventId));
        }

        public IQueryable<Event> GetErrorsByPeriod(DateTime @from, DateTime to)
        {
            // решили что не важно, какая важность у ошибки, нужно её показывать
            // и решили что события компонента это не ошибка

            return Context.Events.Where(x =>
                //x.Importance == EventImportance.Alarm &&
                x.StartDate < to &&
                x.EndDate >= from &&
                x.Category == EventCategory.ApplicationError);
            //&& (x.Category == EventCategory.ApplicationError || x.Category == EventCategory.ComponentEvent));
        }

        public void Remove(Event entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid id, Guid componentId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Event> GetActualClientEvents(Guid componentId, DateTime now)
        {
            var categories = new[]
            {
                EventCategory.ApplicationError,
                EventCategory.ComponentEvent
            };

            return QueryAll(componentId).Where(x => x.ActualDate >= now && categories.Contains(x.Category));
        }

        public IQueryable<Event> GetAllByType(Guid eventTypeId)
        {
            return Context.Events.Where(x => x.EventTypeId == eventTypeId);
        }

        public int DeleteEventParameters(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            var subQuery = GetEventsSubQuery(categories, toDate, maxCount);

            var objectQuery = (ObjectQuery)((IObjectContextAdapter)Context).ObjectContext.CreateObjectSet<EventProperty>()
                .Where(t => subQuery.Contains(t.EventId)).OrderBy(t => t.Id).Select(t => t.Id).Take(maxCount);

            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("EventParameters")} WHERE {Context.FormatColumnName("Id")} IN ({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    return SqlCommandHelper.ExecuteNonQuery(command);
                }
            }
        }

        public int DeleteEvents(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            var objectQuery = (ObjectQuery)GetEventsSubQuery(categories, toDate, maxCount);

            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = $"UPDATE {Context.FormatTableName("Events")} SET {Context.FormatColumnName("LastStatusEventId")} = NULL WHERE {Context.FormatColumnName("LastStatusEventId")} IN({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    SqlCommandHelper.ExecuteNonQuery(command);
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("Events")} WHERE {Context.FormatColumnName("Id")} IN({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    return SqlCommandHelper.ExecuteNonQuery(command);
                }
            }
        }

        public int DeleteEventStatuses(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            var objectQuery = (ObjectQuery)GetEventsSubQuery(categories, toDate, maxCount);

            var count = 0;
            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("EventStatuses")} WHERE {Context.FormatColumnName("EventId")} IN ({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    count += SqlCommandHelper.ExecuteNonQuery(command);
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("EventStatuses")} WHERE {Context.FormatColumnName("StatusId")} IN ({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    count += SqlCommandHelper.ExecuteNonQuery(command);
                }
            }

            return count;
        }

        public int DeleteEventArchivedStatuses(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            var objectQuery = (ObjectQuery)GetEventsSubQuery(categories, toDate, maxCount);

            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("ArchivedStatuses")} WHERE {Context.FormatColumnName("EventId")} IN ({objectQuery.ToTraceString()})";
                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    return SqlCommandHelper.ExecuteNonQuery(command);
                }
            }
        }

        public int UpdateMetricsHistory(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            var objectQuery = (ObjectQuery)GetEventsSubQuery(categories, toDate, maxCount);

            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0;

                    var query = $"UPDATE {Context.FormatTableName("MetricHistory")} SET {Context.FormatColumnName("StatusEventId")} = NULL WHERE {Context.FormatColumnName("StatusEventId")} IN ({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    return SqlCommandHelper.ExecuteNonQuery(command);
                }
            }
        }

        public int DeleteNotifications(EventCategory[] categories, int maxCount, DateTime toDate)
        {
            var subQuery = GetEventsSubQuery(categories, toDate, maxCount);

            using (var connection = Context.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    var objectQuery = (ObjectQuery)((IObjectContextAdapter)Context).ObjectContext.CreateObjectSet<Notification>()
                        .Where(t => subQuery.Contains(t.EventId)).Select(t => t.Id);

                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("NotificationsHttp")} WHERE {Context.FormatColumnName("NotificationId")} IN ({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    SqlCommandHelper.ExecuteNonQuery(command);
                }

                using (var command = connection.CreateCommand())
                {
                    var objectQuery = (ObjectQuery)((IObjectContextAdapter)Context).ObjectContext.CreateObjectSet<Notification>()
                        .Where(t => subQuery.Contains(t.EventId)).Select(t => t.Id);

                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("LastComponentNotifications")} WHERE {Context.FormatColumnName("NotificationId")} IN ({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    SqlCommandHelper.ExecuteNonQuery(command);
                }

                using (var command = connection.CreateCommand())
                {
                    var objectQuery = (ObjectQuery)GetEventsSubQuery(categories, toDate, maxCount);

                    command.CommandTimeout = 0;

                    var query = $"DELETE FROM {Context.FormatTableName("Notifications")} WHERE {Context.FormatColumnName("EventId")} IN ({objectQuery.ToTraceString()})";

                    command.CommandText = query;

                    foreach (var objectParameter in objectQuery.Parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = objectParameter.Name;
                        parameter.Value = objectParameter.Value;
                        command.Parameters.Add(parameter);
                    }

                    return SqlCommandHelper.ExecuteNonQuery(command);
                }
            }
        }

        public int GetEventsCountForDeletion(EventCategory[] categories, DateTime toDate)
        {
            return Context.Events.Count(t => categories.Contains(t.Category) && t.ActualDate < toDate);
        }

        public Event GetRecentReasonEvent(Event statusEvent)
        {
            var recentReasonEvent = GetEventReasons(statusEvent.Id).OrderByDescending(t => t.StartDate).FirstOrDefault();

            if (recentReasonEvent == null)
            {
                return null;
            }

            // список категорий - причин
            var categories = new[]
            {
                EventCategory.ApplicationError,
                EventCategory.ComponentEvent,
                EventCategory.MetricStatus,
                EventCategory.UnitTestResult,
                EventCategory.ComponentExternalStatus
            };

            if (categories.Contains(recentReasonEvent.Category))
            {
                return recentReasonEvent;
            }

            // если это не первопричинное событие, то выполним поиск внутри него
            return GetRecentReasonEvent(recentReasonEvent);
        }

        public TimelineState[] GetTimelineStates(Guid ownerId, EventCategory category, DateTime from, DateTime to)
        {
            var events = QueryAll(ownerId).Where(t => t.Category == category && t.StartDate <= to && t.ActualDate >= from);
            var states = EventsToTimelineStates(events, from, to);
            return states;
        }

        public TimelineState[] GetTimelineStates(Guid ownerId, Guid eventTypeId, DateTime from, DateTime to)
        {
            var events = QueryAll(ownerId).Where(t => t.EventTypeId == eventTypeId && t.StartDate <= to && t.ActualDate >= from);
            var states = EventsToTimelineStates(events, from, to);
            return states;
        }

        public TimelineState[] GetTimelineStatesAnyComponents(Guid eventTypeId, DateTime @from, DateTime to)
        {
            var events = QueryAllByAccount().Where(t => t.EventTypeId == eventTypeId && t.StartDate <= to && t.ActualDate >= from);
            var states = EventsToTimelineStates(events, from, to);
            return states;
        }

        protected TimelineState[] EventsToTimelineStates(IQueryable<Event> events, DateTime from, DateTime to)
        {
            var states = events.Select(t => new TimelineState()
            {
                EventStartDate = t.StartDate,
                EventEndDate = t.EndDate,
                EventActualDate = t.ActualDate,
                Status = t.Importance,
                Message = t.Message,
                Id = t.Id,
                Category = t.Category,
                OwnerId = t.OwnerId,
                Count = t.Count
            }).ToArray();

            foreach (var timelineState in states)
            {
                timelineState.StartDate = timelineState.EventStartDate >= from ? timelineState.EventStartDate : from;
                var realEndDate = timelineState.EventStartDate + EventHelper.GetDuration(timelineState.EventStartDate, timelineState.EventActualDate, DateTime.Now);
                timelineState.EndDate = realEndDate <= to ? realEndDate : to;
            }

            return states;
        }

        public int GetTimelineOkTime(TimelineState[] states, DateTime from, DateTime to)
        {
            var totalTime = (to - from).Ticks;
            var failStates = states.Where(t => t.Status == EventImportance.Alarm || t.Status == EventImportance.Warning).ToArray();
            var failTime = failStates.Sum(t => (t.EndDate - t.StartDate).Ticks);
            var notFailTime = Math.Max(0, totalTime - failTime);
            var okTime = (int)(notFailTime * 100 / (totalTime != 0 ? totalTime : 1));
            return okTime;
        }

        public Event GetLastEventByEndDate(Guid eventTypeId)
        {
            return QueryAllByAccount()
                    .Where(x => x.EventTypeId == eventTypeId)
                    .OrderByDescending(x => x.EndDate)
                    .FirstOrDefault();
        }

        private IQueryable<Guid> GetEventsSubQuery(EventCategory[] categories, DateTime actualDate, int maxCount)
        {
            return ((IObjectContextAdapter)Context).ObjectContext.CreateObjectSet<Event>()
                .Where(t => categories.Contains(t.Category) && t.ActualDate < actualDate)
                .OrderBy(t => t.Category)
                .ThenBy(t => t.ActualDate)
                .Select(t => t.Id)
                .Take(maxCount);
        }

    }
}
