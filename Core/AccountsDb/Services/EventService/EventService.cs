using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Common;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.DispatcherLayer;
using Zidium.Core.Limits;

namespace Zidium.Core.AccountsDb
{
    public class EventService : IEventService
    {
        protected DispatcherContext Context { get; set; }

        public EventService(DispatcherContext dispatcherContext)
        {
            if (dispatcherContext == null)
            {
                throw new ArgumentNullException("dispatcherContext");
            }
            Context = dispatcherContext;
        }

        /* НЕ ИСПОЛЬЗУЕТСЯ
        /// <summary>
        /// Возвращает последнее открытое событие для склейки
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="componentId">Ссылка на компонент</param>
        /// <param name="eventType"></param>
        /// <param name="joinKeyHash">Хэш ключа склейки</param>
        /// <returns>Событие из БД для склейки</returns>
        protected Event GetLastSimpleEvent(
            Guid accountId,
            Guid componentId,
            EventType eventType,
            long joinKeyHash)
        {
            var storageDbContext = Context.DbContext.GetStorageDbContextByAccountId(accountId);
            var eventFromDb = storageDbContext
                .Events
                .Where(x => x.OwnerId == componentId
                            && x.EventTypeId == eventType.Id
                            && x.JoinKeyHash == joinKeyHash)
                .OrderByDescending(x => x.EndDate)
                .FirstOrDefault();

            return eventFromDb;
        }
        */

        protected int AddCount(int value, int add)
        {
            long result = (long)value + add;
            if (result > Int32.MaxValue)
            {
                return Int32.MaxValue;
            }
            return (int)result;
        }

        protected void UpdateJoinedEvent(Guid accountId, Guid componentId, EventCacheWriteObject oldEvent, Event newEvent)
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

        protected IEventCacheReadObject CreateOrJoinSimpleEvent(
            Guid componentId,
            Event eventObj,
            Guid? oldEventId,
            TimeSpan joinInterval,
            Guid accountId,
            out bool isEventNew)
        {
            IEventCacheReadObject rLastEvent = null;
            if (oldEventId.HasValue)
            {
                rLastEvent = AllCaches.Events.Find(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = oldEventId.Value
                });
            }
            else
            {
                var accountDbContext = Context.DbContext.GetAccountDbContext(accountId);
                var repository = accountDbContext.GetEventRepository();
                var lastEvent = repository.GetForJoin(eventObj);
                if (lastEvent != null)
                {
                    rLastEvent = AllCaches.Events.GetForRead(lastEvent, accountId);
                }
            }

            if (rLastEvent != null)
            {
                var request = new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = rLastEvent.Id
                };
                if (AllCaches.Events.ExistsInStorage(request))
                {
                    using (var wLastEvent = AllCaches.Events.Write(request))
                    {
                        bool canJoin = EventHelper.CanJoinSimpleEvents(wLastEvent, eventObj, joinInterval);
                        if (canJoin)
                        {
                            // Можно склеивать
                            UpdateJoinedEvent(accountId, componentId, wLastEvent, eventObj);
                            isEventNew = false;
                            wLastEvent.BeginSave();
                            return wLastEvent;
                        }
                    }
                }
            }


            // Создадим новое
            {
                var accountDbContext = Context.DbContext.GetAccountDbContext(accountId);
                var eventRepository = accountDbContext.GetEventRepository();
                isEventNew = true;
                eventRepository.Add(eventObj);
                accountDbContext.SaveChanges();
                var wEvent = EventCacheWriteObject.CreateForUpdate(eventObj, accountId);
                AllCaches.Events.AddOrGet(wEvent);
                return wEvent;
            }
        }

        public IEventCacheReadObject GetEventCacheOrNullById(Guid accountId, Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                return null;
            }
            var eventObj = AllCaches.Events.Find(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = eventId
            });
            return eventObj;
        }

        public Event GetEventOrNullById(Guid accountId, Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                return null;
            }
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetEventRepository();
            var eventObj = repository.GetByIdOrNull(eventId);
            return eventObj;
        }

        public Event GetEventById(Guid accountId, Guid eventId)
        {
            var eventObj = GetEventOrNullById(accountId, eventId);
            if (eventObj == null)
                throw new UnknownEventIdException(eventId);
            return eventObj;
        }

        protected void FixEventMessageTypeCode(SendEventData eventMessage)
        {
            if (eventMessage.TypeCode != null && eventMessage.TypeCode.Length > 20)
            {
                eventMessage.TypeCode = eventMessage.TypeCode.Substring(0, 20);
            }
        }

        protected void FixEventMessageNames(SendEventData eventMessage)
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

        protected void FixEventMessageJoinKey(SendEventData message)
        {
            if (message.JoinKey == null)
            {
                if (message.Category == EventCategory.ApplicationError)
                {
                    var stack = message.Properties.GetValue(ExtentionPropertyName.Stack);
                    message.JoinKey = HashHelper.GetInt64(message.TypeSystemName, stack);
                }
            }
        }

        protected void FixEventMessageJoinInterval(SendEventData message)
        {
            if (message.JoinInterval == null)
            {
                if (message.Category == EventCategory.ApplicationError)
                {
                    message.JoinInterval = TimeSpan.FromDays(1).TotalSeconds;
                }
                else if (message.Category == EventCategory.ComponentEvent)
                {
                    message.JoinInterval = 0;
                }
            }
        }

        protected EventImportance GetEventImportance(EventImportance? importance, EventCategory category)
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

        protected EventType GetOrCreateEventType(
            Guid accountId,
            EventCategory category,
            string systemName,
            string displayName,
            Guid componentTypeId,
            string typeCode)
        {
            var type = new EventType()
            {
                Category = category,
                SystemName = systemName,
                DisplayName = displayName,
                Code = typeCode
            };
            type = Context.EventTypeService.GetOrCreate(type, accountId);
            return type;
        }

        protected Event CreateEvent(
            Guid accountId,
            DateTime? startDate,
            DateTime? endDate,
            int? count,
            string version,
            EventImportance? importance,
            TimeSpan? actualTime,
            string message,
            List<Api.ExtentionPropertyDto> properties,
            EventType eventType,
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
            var result = new Event()
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
                Properties = ApiConverter.GetEventProperties(properties),
                IsUserHandled = false,
                VersionLong = VersionHelper.FromString(version)
            };
            return result;
        }

        protected bool EventIsOld(string eventTypeOldVersion, string version)
        {
            var oldVersion = VersionHelper.FromString(eventTypeOldVersion);
            var eventVersion = VersionHelper.FromString(version);
            return eventVersion <= oldVersion;
        }

        protected void CheckEventStartDate(SendEventData message, Guid accountId, Guid eventTypeId, Guid eventId)
        {
            if (message.StartDate > DateTime.Now.AddMinutes(5))
            {
                var now = DateTime.Now;
                try
                {
                    var eventData = new SendEventData()
                    {
                        JoinInterval = TimeSpan.FromHours(1).TotalSeconds,
                        Category = EventCategory.ComponentEvent,
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
                        Properties = new List<Api.ExtentionPropertyDto>()
                    };
                    eventData.Properties.AddValue("EventId", eventId);
                    eventData.Properties.AddValue("EventTypeId", eventTypeId);
                    eventData.Properties.AddValue("EventMessage", message.Message);
                    SendEvent(accountId, message.ComponentId ?? Guid.Empty, eventData);
                }
                catch (Exception)
                {
                    // todo Что тут должно быть?
                }
                throw new ResponseCodeException(
                    Zidium.Api.ResponseCode.FutureEvent,
                    "Время начала события значительно больше, чем текущее время");
            }
        }

        public IEventCacheReadObject SendEvent(Guid accountId, Guid componentId, SendEventData message)
        {
            // Проверим, что компонент вообще есть
            var componentRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = componentId
            };
            var component = AllCaches.Components.Find(componentRequest);
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId, accountId);
            }
            if (component.CanProcess == false)
            {
                throw new ResponseCodeException(Zidium.Api.ResponseCode.ObjectDisabled, "Компонент выключен");
            }

            var accountDbContext = Context.GetAccountDbContext(accountId);
            var limitChecker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            var size = message.GetSize();

            try
            {
                // проверка лимитов
                limitChecker.CheckEventsRequestsPerDay(accountDbContext);

                // Проверим лимит размера хранилища
                limitChecker.CheckStorageSize(accountDbContext, size);

                // todo нет проверки обязательных параметров
                FixEventMessageNames(message);
                FixEventMessageTypeCode(message);
                FixEventMessageJoinKey(message); // todo не уверен, что нужно менять ключ склейки при разных версиях (но склеивать с  разными версиями нельзя!)
                FixEventMessageJoinInterval(message);

                // категория
                if (message.Category == null)
                {
                    message.Category = EventCategory.ComponentEvent;
                }

                // важность
                message.Importance = GetEventImportance(message.Importance, message.Category.Value);

                if (message.StartDate == null)
                {
                    message.StartDate = DateTime.Now;
                }

                var eventType = GetOrCreateEventType(
                    accountId,
                    message.Category.Value,
                    message.TypeSystemName,
                    message.TypeDisplayName,
                    component.ComponentTypeId,
                    message.TypeCode);

                var joinTime = DataConverter.GetTimeSpanFromSeconds(message.JoinInterval);

                var localEvent = CreateEvent(
                    accountId,
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
                var joinInterval = eventType.JoinInterval ?? joinTime ?? TimeSpan.Zero;

                if (!EventCategoryHelper.IsCustomerEventCategory(eventType.Category))
                    throw new Exception("Недопустимая категория события: " + eventType.Category);

                var result = ProcessSimpleEvent(componentId, localEvent, null, joinInterval, accountId);

                // если есть закрытый или тестируемый дефект и событие - новая ошибка, то переоткроем дефект
                if (eventType.Defect != null)
                {
                    var defectStatus = eventType.Defect.GetStatus();
                    if ((defectStatus == DefectStatus.Closed || defectStatus == DefectStatus.Testing) && !EventIsOld(eventType.OldVersion, result.Version))
                    {
                        var defectService = accountDbContext.GetDefectService();
                        defectService.AutoReopen(accountId, eventType.Defect);
                        accountDbContext.SaveChanges();
                    }
                }

                // проверим "событие из будущего"
                CheckEventStartDate(message, accountId, eventType.Id, result.Id);

                return result;
            }
            finally
            {
                limitChecker.AddEventsRequestsPerDay(accountDbContext);
                limitChecker.AddEventsSizePerDay(accountDbContext, size);
            }
        }

        protected IEventCacheReadObject ProcessSimpleEvent(
            Guid componentId,
            Event eventObj,
            Guid? oldEventId,
            TimeSpan joinInterval,
            Guid accountId)
        {
            // используем блокировку, чтобы потокобезопасно склеивать события на стороне диспетчера
            var lockObj = LockObject.ForProcessSimpleEvent(componentId, eventObj.EventTypeId, eventObj.JoinKeyHash);
            lock (lockObj)
            {
                // склеиваем
                bool isEventNew;
                var rEvent = CreateOrJoinSimpleEvent(componentId, eventObj, oldEventId, joinInterval, accountId, out isEventNew);

                // обновим статус компонента
                if (rEvent != null)
                {
                    Context.ComponentService.ProcessEvent(accountId, componentId, rEvent);
                }
                return rEvent;
            }
        }

        public IEventCacheReadObject JoinEvent(Guid accountId, Guid componentId, JoinEventData joinEventData)
        {
            // Проверим, что компонент вообще есть
            var componentRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = componentId
            };
            var component = AllCaches.Components.Find(componentRequest);
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId, accountId);
            }
            if (component.CanProcess == false)
            {
                throw new ResponseCodeException(Zidium.Api.ResponseCode.ObjectDisabled, "Компонент выключен");
            }

            // проверка лимитов
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var limitChecker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            limitChecker.CheckEventsRequestsPerDay(accountDbContext);

            // Проверим лимит размера хранилища
            var size = joinEventData.Message != null ? joinEventData.Message.Length * 2 : 0;
            limitChecker.CheckStorageSize(accountDbContext, size);

            var eventType = Context.EventTypeService.GetOneById(joinEventData.TypeId ?? Guid.Empty, accountId);

            // расчитаем важность
            joinEventData.Importance = GetEventImportance(joinEventData.Importance, eventType.Category);

            var localEvent = CreateEvent(
                accountId,
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
                : (eventType.JoinInterval ?? TimeSpan.Zero);

            var dbEvent = ProcessSimpleEvent(componentId, localEvent, joinEventData.EventId, joinInterval, accountId);

            limitChecker.AddEventsSizePerDay(accountDbContext, size);

            return dbEvent;
        }

        public List<Event> GetEvents(Guid accountId, GetEventsRequestData filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            Guid? eventTypeId = null;
            if (string.IsNullOrEmpty(filter.TypeSystemName) == false)
            {
                var eventType = Context.EventTypeService.GetOneBySystemName(accountId, filter.TypeSystemName);
                eventTypeId = eventType.Id;
            }

            var maxCount = filter.MaxCount ?? 1000;
            if (maxCount > 1000)
                maxCount = 1000;

            var accountDbContext = Context.GetAccountDbContext(accountId);
            var eventRepository = accountDbContext.GetEventRepository();

            var events = eventRepository.QueryAll(filter.OwnerId.Value,
                filter.Category,
                filter.From,
                filter.To,
                filter.Importance,
                eventTypeId,
                filter.SearthText,
                maxCount).ToList();

            return events;
        }

        public Event GetDangerousEvent(
            Guid accountId,
            Guid componentId,
            DateTime fromDate)
        {
            var categories = new List<EventCategory>
            {
                EventCategory.ApplicationError,
                EventCategory.ComponentEvent
            };

            var accountDbContext = Context.GetAccountDbContext(accountId);

            // берем самое опасное событие, которое длилось в указанную дату
            var eventId = accountDbContext.Events
                .Where(x =>
                    x.OwnerId == componentId
                    && categories.Contains(x.Category)
                    && x.StartDate <= fromDate
                    && x.ActualDate > fromDate
                    )
                .OrderByDescending(x => x.Importance)
                .ThenByDescending(x => x.ActualDate) // из всех опасных возьмем самое длинное
                .Select(t => t.Id)
                .Take(1)
                .FirstOrDefault();

            return eventId != Guid.Empty ? accountDbContext.Events.Find(eventId) : null;
        }

        public Event Update(Guid accountId, Event _event)
        {
            _event.LastUpdateDate = DateTime.Now;
            var accountDbContext = Context.GetAccountDbContext(accountId);
            accountDbContext.SaveChanges();
            return _event;
        }

        public void Add(Guid accountId, Event eventObj)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetEventRepository();
            repository.Add(eventObj);
            accountDbContext.SaveChanges();
        }
    }
}
