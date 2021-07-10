using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Common;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.AccountDb;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.Limits;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class EventService : IEventService
    {
        public EventService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        private int AddCount(int value, int add)
        {
            var result = (long)value + add;
            if (result > int.MaxValue)
            {
                return int.MaxValue;
            }
            return (int)result;
        }

        private void UpdateJoinedEvent(EventCacheWriteObject oldEvent, EventForAdd newEvent)
        {
            oldEvent.Importance = newEvent.Importance;
            oldEvent.Count = AddCount(oldEvent.Count, newEvent.Count);
            oldEvent.ActualDate = newEvent.ActualDate;
            oldEvent.Message = newEvent.Message; // сообщение обновлять надо, чтобы в личном кабинете можно было смотреть последнее сообщение проверки
            if (newEvent.EndDate > oldEvent.EndDate)
            {
                oldEvent.EndDate = newEvent.EndDate;
            }
            oldEvent.LastUpdateDate = DateTime.Now;
        }

        private IEventCacheReadObject CreateOrJoinSimpleEvent(
            Guid componentId,
            EventForAdd eventInfo,
            Guid? oldEventId,
            TimeSpan joinInterval,
            out bool isEventNew)
        {
            IEventCacheReadObject rLastEvent = null;
            if (oldEventId.HasValue)
            {
                rLastEvent = AllCaches.Events.Find(new AccountCacheRequest()
                {
                    ObjectId = oldEventId.Value
                });
            }
            else
            {
                var lastEvent = _storage.Events.GetForJoin(eventInfo);
                if (lastEvent != null)
                {
                    rLastEvent = AllCaches.Events.GetForRead(lastEvent.Id);
                }
            }

            if (rLastEvent != null)
            {
                var request = new AccountCacheRequest()
                {
                    ObjectId = rLastEvent.Id
                };
                if (AllCaches.Events.ExistsInStorage(request))
                {
                    using (var wLastEvent = AllCaches.Events.Write(request))
                    {
                        var canJoin = EventHelper.CanJoinSimpleEvents(wLastEvent, eventInfo, joinInterval);
                        if (canJoin)
                        {
                            // Можно склеивать
                            UpdateJoinedEvent(wLastEvent, eventInfo);
                            isEventNew = false;
                            wLastEvent.BeginSave();
                            return wLastEvent;
                        }
                    }
                }
            }


            // Создадим новое
            isEventNew = true;

            _storage.Events.Add(eventInfo);

            var wEvent = AllCaches.Events.GetForRead(eventInfo.Id);
            return wEvent;
        }

        public IEventCacheReadObject GetEventCacheOrNullById(Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                return null;
            }
            var eventObj = AllCaches.Events.Find(new AccountCacheRequest()
            {
                ObjectId = eventId
            });
            return eventObj;
        }

        private void FixEventMessageTypeCode(SendEventRequestDataDto eventMessage)
        {
            if (eventMessage.TypeCode != null && eventMessage.TypeCode.Length > 20)
            {
                eventMessage.TypeCode = eventMessage.TypeCode.Substring(0, 20);
            }
        }

        private void FixEventMessageNames(SendEventRequestDataDto eventMessage)
        {
            // поправим имена
            if (eventMessage.TypeSystemName == null)
            {
                eventMessage.TypeSystemName = eventMessage.TypeDisplayName;
            }
            if (eventMessage.TypeDisplayName == null)
            {
                eventMessage.TypeDisplayName = eventMessage.TypeSystemName;
            }
            if (string.IsNullOrEmpty(eventMessage.TypeSystemName))
            {
                throw new ParameterRequiredException("TypeSystemName");
            }

            if (eventMessage.TypeSystemName.Length > 255)
                eventMessage.TypeSystemName = eventMessage.TypeSystemName.Substring(0, 255);

            if (eventMessage.TypeDisplayName.Length > 255)
                eventMessage.TypeDisplayName = eventMessage.TypeDisplayName.Substring(0, 255);
        }

        private void FixEventMessageJoinKey(SendEventRequestDataDto message)
        {
            if (message.JoinKey == null)
            {
                if (message.Category == SendEventCategory.ApplicationError)
                {
                    var stack = message.Properties.GetValue(ExtentionPropertyName.Stack);
                    message.JoinKey = HashHelper.GetInt64(message.TypeSystemName, stack);
                }
            }
        }

        private void FixEventMessageJoinInterval(SendEventRequestDataDto message)
        {
            if (message.JoinIntervalSeconds == null)
            {
                if (message.Category == SendEventCategory.ApplicationError)
                {
                    message.JoinIntervalSeconds = TimeSpan.FromDays(1).TotalSeconds;
                }
                else if (message.Category == SendEventCategory.ComponentEvent)
                {
                    message.JoinIntervalSeconds = 0;
                }
            }
        }

        private EventImportance GetEventImportance(EventImportance? importance, EventCategory category)
        {
            if (importance.HasValue)
            {
                return importance.Value;
            }
            if (category == EventCategory.ApplicationError)
            {
                return EventImportance.Alarm;
            }
            return EventImportance.Unknown;
        }

        private EventTypeForRead GetOrCreateEventType(
            EventCategory category,
            string systemName,
            string displayName,
            string typeCode)
        {
            var eventTypeForAdd = new EventTypeForAdd()
            {
                Category = category,
                SystemName = systemName,
                DisplayName = displayName,
                Code = typeCode
            };
            var eventTypeService = new EventTypeService(_storage);
            var eventType = eventTypeService.GetOrCreate(eventTypeForAdd);
            return eventType;
        }

        private EventForAdd CreateEvent(
            DateTime? startDate,
            DateTime? endDate,
            int? count,
            string version,
            EventImportance? importance,
            TimeSpan? actualTime,
            string message,
            List<ExtentionPropertyDto> properties,
            EventTypeForRead eventType,
            Guid componentId,
            long joinKeyHash)
        {
            // вычисляем значения по умолчанию, если они не указаны явно
            startDate = startDate ?? DateTime.Now;
            endDate = endDate ?? startDate;
            count = count ?? 1;
            if (count < 1)
            {
                count = 1;
            }

            if (EventIsOld(eventType.OldVersion, version))
            {
                importance = eventType.ImportanceForOld ?? importance;
            }
            else
            {
                importance = eventType.ImportanceForNew ?? importance;
            }

            var now = DateTime.Now;
            var result = new EventForAdd()
            {
                Id = Guid.NewGuid(),
                EventTypeId = eventType.Id,
                Importance = importance ?? EventImportance.Unknown,
                CreateDate = now,
                OwnerId = componentId,
                Count = count.Value,
                EndDate = endDate.Value,
                LastUpdateDate = now,
                LastNotificationDate = null,
                JoinKeyHash = joinKeyHash,
                StartDate = startDate.Value,
                ActualDate = startDate.Value + (actualTime ?? TimeSpan.Zero),
                Message = message,
                Version = version,
                Category = eventType.Category,
                IsUserHandled = false,
                VersionLong = VersionHelper.FromString(version),
                Properties = properties != null ? ApiConverter.GetEventProperties(properties) : new EventPropertyForAdd[0]
            };

            foreach (var property in result.Properties)
            {
                property.EventId = result.Id;
            }

            return result;
        }

        protected bool EventIsOld(string eventTypeOldVersion, string version)
        {
            var oldVersion = VersionHelper.FromString(eventTypeOldVersion);
            var eventVersion = VersionHelper.FromString(version);
            return eventVersion <= oldVersion;
        }

        protected void CheckEventStartDate(SendEventRequestDataDto message, Guid eventTypeId, Guid eventId)
        {
            if (message.StartDate > DateTime.Now.AddMinutes(5))
            {
                var now = DateTime.Now;
                try
                {
                    var eventData = new SendEventRequestDataDto()
                    {
                        JoinIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds,
                        Category = SendEventCategory.ComponentEvent,
                        Count = 1,
                        StartDate = now,
                        Importance = EventImportance.Warning,
                        Message = string.Format(
                            "Время начала события: {0}, текущее время: {1}",
                            (message.StartDate ?? now).ToString("dd.MM.yyyy HH:mm:ss"),
                            now.ToString("dd.MM.yyyy HH:mm:ss")),
                        TypeSystemName = "System.FutureEvent",
                        TypeDisplayName = "Время начала события значительно больше, чем текущее время",
                        Version = message.Version,
                        Properties = new List<ExtentionPropertyDto>()
                    };
                    eventData.Properties.AddValue("EventId", eventId);
                    eventData.Properties.AddValue("EventTypeId", eventTypeId);
                    eventData.Properties.AddValue("EventMessage", message.Message);
                    SendEvent(message.ComponentId ?? Guid.Empty, eventData);
                }
                catch (Exception)
                {
                    // TODO Что тут должно быть?
                }
                throw new ResponseCodeException(
                    ResponseCode.FutureEvent,
                    "Время начала события значительно больше, чем текущее время");
            }
        }

        public IEventCacheReadObject SendEvent(Guid componentId, SendEventRequestDataDto message)
        {
            // Проверим, что компонент вообще есть
            var componentRequest = new AccountCacheRequest()
            {
                ObjectId = componentId
            };
            var component = AllCaches.Components.Find(componentRequest);
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId);
            }
            if (component.CanProcess == false)
            {
                throw new ResponseCodeException(ResponseCode.ObjectDisabled, "Компонент выключен");
            }

            var limitChecker = AccountLimitsCheckerManager.GetChecker();

            try
            {
                // todo нет проверки обязательных параметров
                FixEventMessageNames(message);
                FixEventMessageTypeCode(message);
                FixEventMessageJoinKey(message); // todo не уверен, что нужно менять ключ склейки при разных версиях (но склеивать с  разными версиями нельзя!)
                FixEventMessageJoinInterval(message);
                message.Message = message.Message.FixStringSymbols();

                if (message.Properties != null)
                {
                    foreach (var eventProperty in message.Properties)
                    {
                        eventProperty.Name = eventProperty.Name.FixStringSymbols();
                        eventProperty.Value = eventProperty.Value.FixStringSymbols();
                    }
                }

                // категория
                if (message.Category == null)
                {
                    message.Category = SendEventCategory.ComponentEvent;
                }

                // важность
                message.Importance = GetEventImportance(message.Importance, (EventCategory)message.Category.Value);

                if (message.StartDate == null)
                {
                    message.StartDate = DateTime.Now;
                }

                var size = message.GetSize();

                try
                {

                    var eventType = GetOrCreateEventType(
                        (EventCategory)message.Category.Value,
                        message.TypeSystemName,
                        message.TypeDisplayName,
                        message.TypeCode);

                    var joinTime = DataConverter.GetTimeSpanFromSeconds(message.JoinIntervalSeconds);

                    var localEvent = CreateEvent(
                        message.StartDate,
                        message.StartDate,
                        message.Count,
                        message.Version,
                        message.Importance,
                        TimeSpan.FromMinutes(1),
                        message.Message,
                        message.Properties,
                        eventType,
                        componentId,
                        message.JoinKey ?? 0);

                    // eventType.JoinInterval стоит на первом месте, чтобы можно было изменить поведение БЕЗ перекомпиляции кода
                    var joinInterval = eventType.JoinInterval() ?? joinTime ?? TimeSpan.Zero;

                    if (!EventCategoryHelper.IsCustomerEventCategory(eventType.Category))
                        throw new Exception("Недопустимая категория события: " + eventType.Category);

                    var result = ProcessSimpleEvent(componentId, localEvent, null, joinInterval);

                    // если есть закрытый или тестируемый дефект и событие - новая ошибка, то переоткроем дефект
                    if (eventType.DefectId != null)
                    {
                        var defectService = new DefectService(_storage);
                        var defectStatus = defectService.GetStatus(eventType.DefectId.Value);
                        if ((defectStatus == DefectStatus.Closed || defectStatus == DefectStatus.Testing) && !EventIsOld(eventType.OldVersion, result.Version))
                        {
                            var defect = _storage.Defects.GetOneById(eventType.DefectId.Value);
                            defectService.AutoReopen(defect);
                        }
                    }

                    // проверим "событие из будущего"
                    CheckEventStartDate(message, eventType.Id, result.Id);

                    return result;
                }
                finally
                {
                    limitChecker.AddEventsSizePerDay(_storage, size);
                }
            }
            finally
            {
                limitChecker.AddEventsRequestsPerDay(_storage);
            }
        }

        private IEventCacheReadObject ProcessSimpleEvent(
            Guid componentId,
            EventForAdd eventInfo,
            Guid? oldEventId,
            TimeSpan joinInterval)
        {
            // используем блокировку, чтобы потокобезопасно склеивать события на стороне диспетчера
            var lockObj = LockObject.ForProcessSimpleEvent(componentId, eventInfo.EventTypeId, eventInfo.JoinKeyHash);
            lock (lockObj)
            {
                // склеиваем
                var rEvent = CreateOrJoinSimpleEvent(componentId, eventInfo, oldEventId, joinInterval, out _);

                // обновим статус компонента
                if (rEvent != null)
                {
                    var componentService = new ComponentService(_storage);
                    componentService.ProcessEvent(componentId, rEvent);
                }
                return rEvent;
            }
        }

        public IEventCacheReadObject JoinEvent(Guid componentId, JoinEventRequestDataDto joinEventData)
        {
            // Проверим, что компонент вообще есть
            var componentRequest = new AccountCacheRequest()
            {
                ObjectId = componentId
            };
            var component = AllCaches.Components.Find(componentRequest);
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId);
            }
            if (component.CanProcess == false)
            {
                throw new ResponseCodeException(ResponseCode.ObjectDisabled, "Компонент выключен");
            }

            var limitChecker = AccountLimitsCheckerManager.GetChecker();
            var size = joinEventData.Message != null ? joinEventData.Message.Length * 2 : 0;

            try
            {
                var eventTypeService = new EventTypeService(_storage);
                var eventType = eventTypeService.GetOneById(joinEventData.TypeId ?? Guid.Empty);

                // расчитаем важность
                joinEventData.Importance = GetEventImportance(joinEventData.Importance, eventType.Category);

                var localEvent = CreateEvent(
                    joinEventData.StartDate,
                    joinEventData.StartDate,
                    joinEventData.Count,
                    joinEventData.Version,
                    joinEventData.Importance,
                    TimeSpan.FromSeconds(joinEventData.JoinInterval ?? 0),
                    joinEventData.Message,
                    null,
                    eventType,
                    componentId,
                    joinEventData.JoinKey ?? 0);

                var joinInterval = (joinEventData.JoinInterval.HasValue)
                    ? TimeSpan.FromSeconds(joinEventData.JoinInterval.Value)
                    : (eventType.JoinInterval() ?? TimeSpan.Zero);

                var dbEvent = ProcessSimpleEvent(componentId, localEvent, joinEventData.EventId, joinInterval);

                return dbEvent;
            }
            finally
            {
                limitChecker.AddEventsSizePerDay(_storage, size);
            }
        }

        public EventForRead[] GetEvents(GetEventsRequestDataDto filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            if (filter.OwnerId == null)
            {
                throw new ArgumentNullException("filter.OwnerId");
            }

            Guid? eventTypeId = null;
            if (string.IsNullOrEmpty(filter.TypeSystemName) == false)
            {
                var eventTypeService = new EventTypeService(_storage);
                var eventType = eventTypeService.GetOneBySystemName(filter.TypeSystemName);
                eventTypeId = eventType.Id;
            }

            var maxCount = filter.MaxCount ?? 1000;
            if (maxCount > 1000)
                maxCount = 1000;

            var events = _storage.Events.Filter(filter.OwnerId.Value,
                filter.Category,
                filter.From,
                filter.To,
                filter.Importance,
                eventTypeId,
                filter.SearchText,
                maxCount);

            return events;
        }

        public EventForRead GetDangerousEvent(
            Guid componentId,
            DateTime fromDate)
        {
            // TODO To static field
            var categories = new[]
            {
                EventCategory.ApplicationError,
                EventCategory.ComponentEvent
            };

            // берем самое опасное событие, которое длилось в указанную дату
            return _storage.Events.GetMostDangerousEvent(componentId, categories, fromDate);
        }

        public void Update(EventForUpdate eventObj)
        {
            eventObj.LastUpdateDate.Set(DateTime.Now);
            _storage.Events.Update(eventObj);
        }

        public void Add(EventForAdd eventObj)
        {
            _storage.Events.Add(eventObj);
        }

        public EventForRead GetRecentReasonEvent(EventForRead statusEvent)
        {
            var recentReasonEvent = _storage.Events.GetRecentEventReason(statusEvent.Id);

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
            var events = _storage.Events.Filter(ownerId, category, from, to);
            var states = EventsToTimelineStates(events, from, to);
            return states;
        }

        public TimelineState[] GetTimelineStates(Guid ownerId, Guid eventTypeId, DateTime from, DateTime to)
        {
            var events = _storage.Events.Filter(ownerId, eventTypeId, from, to);
            var states = EventsToTimelineStates(events, from, to);
            return states;
        }

        public TimelineState[] GetTimelineStatesAnyComponents(Guid eventTypeId, DateTime @from, DateTime to)
        {
            var events = _storage.Events.Filter(eventTypeId, from, to);
            var states = EventsToTimelineStates(events, from, to);
            return states;
        }

        protected TimelineState[] EventsToTimelineStates(EventForRead[] events, DateTime from, DateTime to)
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

    }
}
