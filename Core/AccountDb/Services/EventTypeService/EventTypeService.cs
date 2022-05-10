using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class EventTypeService : IEventTypeService
    {
        public EventTypeService(IStorage storage, ITimeService timeService)
        {
            _storage = storage;
            _timeService = timeService;
        }

        private readonly IStorage _storage;
        private readonly ITimeService _timeService;

        public EventTypeForRead GetOneById(Guid eventTypeId)
        {
            return _storage.EventTypes.GetOneById(eventTypeId);
        }

        public EventTypeForRead GetOrCreate(EventTypeForAdd eventType)
        {
            var lockObj = LockObject.ForEventType(eventType.SystemName);
            lock (lockObj)
            {
                EventTypeForRead result;
                if (eventType.Id == Guid.Empty)
                {
                    result = _storage.EventTypes.GetOneOrNullByCategoryAndName(eventType.Category, eventType.SystemName);
                }
                else
                {
                    result = _storage.EventTypes.GetOneOrNullById(eventType.Id);
                }

                if (result == null)
                {
                    if (eventType.Id == Guid.Empty)
                    {
                        eventType.Id = Ulid.NewUlid();
                    }
                    eventType.CreateDate = _timeService.Now();
                    _storage.EventTypes.Add(eventType);
                    result = _storage.EventTypes.GetOneById(eventType.Id);
                }
                else
                {
                    // укажем код, если его раньше не было
                    if (result.Code == null && eventType.Code != null)
                    {
                        var eventTypeForUpdate = result.GetForUpdate();
                        eventTypeForUpdate.Code.Set(eventType.Code);
                        _storage.EventTypes.Update(eventTypeForUpdate);
                    }
                }
                return result;
            }
        }

        public EventTypeForRead GetOneBySystemName(string systemName)
        {
            var type = _storage.EventTypes.GetOneOrNullBySystemName(systemName);
            if (type == null)
            {
                throw new Exception("Не удалось найти тип события " + systemName);
            }
            return type;
        }

        public void Update(UpdateEventTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var eventType = _storage.EventTypes.GetOneById(data.EventTypeId);

            if (eventType.IsSystem)
            {
                throw new UserFriendlyException("Нельзя менять системный тип события");
            }

            var eventTypeForUpdate = eventType.GetForUpdate();
            eventTypeForUpdate.OldVersion.Set(data.OldVersion);
            eventTypeForUpdate.ImportanceForOld.Set(data.ImportanceForOld);
            eventTypeForUpdate.ImportanceForNew.Set(data.ImportanceForNew);
            eventTypeForUpdate.JoinIntervalSeconds.Set(data.JoinIntervalSeconds);

            if (!string.IsNullOrEmpty(data.DisplayName))
            {
                eventTypeForUpdate.DisplayName.Set(data.DisplayName);
            }
            if (string.IsNullOrEmpty(data.SystemName) == false)
            {
                eventTypeForUpdate.SystemName.Set(data.SystemName);
            }

            _storage.EventTypes.Update(eventTypeForUpdate);

            if (data.UpdateActualEvents == false)
            {
                return;
            }

            eventType = _storage.EventTypes.GetOneById(eventType.Id);

            // Обновим старые события
            var oldVersion = VersionHelper.FromString(eventType.OldVersion);
            var now = _timeService.Now();
            var components = new List<Guid>();

            var events = _storage.Events.GetActualByType(eventType.Id, now).ToList();

            // если есть событие - инициатор (на основании которого решили изменить важность)
            if (data.EventId.HasValue)
            {
                var eventObj = _storage.Events.GetOneById(data.EventId.Value);
                events.Add(eventObj);
            }

            // обновим события
            foreach (var _event in events)
            {
                if (eventType.ImportanceForOld.HasValue && _event.VersionLong <= oldVersion)
                {
                    // ставим важность для старых событий
                    if (_event.Importance != eventType.ImportanceForOld.Value)
                    {
                        var eventForUpdate = _event.GetForUpdate();
                        eventForUpdate.Importance.Set(eventType.ImportanceForOld.Value);
                        _storage.Events.Update(eventForUpdate);
                        components.Add(_event.OwnerId);
                    }
                }
                else if (eventType.ImportanceForNew.HasValue &&
                         (oldVersion == null || _event.VersionLong > oldVersion))
                {
                    // ставим важность для новых событий
                    if (_event.Importance != eventType.ImportanceForNew.Value)
                    {
                        var eventForUpdate = _event.GetForUpdate();
                        eventForUpdate.Importance.Set(eventType.ImportanceForNew.Value);
                        _storage.Events.Update(eventForUpdate);
                        components.Add(_event.OwnerId);
                    }
                }
            }

            // выгрузим события из кэша
            foreach (var _event in events)
            {
                AllCaches.Events.Unload(new AccountCacheRequest()
                {
                    ObjectId = _event.Id
                });
            }

            // обновим статусы компонентов
            // TODO DI
            var componentService = new ComponentService(_storage, _timeService);
            var componentIds = components.Distinct().ToList();
            foreach (var id in componentIds)
            {
                componentService.CalculateEventsStatus(id);
            }

        }
    }
}
