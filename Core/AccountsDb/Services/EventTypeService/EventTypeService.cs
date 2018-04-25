using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.DispatcherLayer;

namespace Zidium.Core.AccountsDb
{
    public class EventTypeService : IEventTypeService
    {
        protected DispatcherContext Context { get; set; }

        protected IEventTypeRepository GetEventTypeRepository(Guid accountId)
        {
            return Context.DbContext.GetAccountDbContext(accountId).GetEventTypeRepository();
        }

        public EventTypeService(DispatcherContext dispatcherContext)
        {
            if (dispatcherContext == null)
            {
                throw new ArgumentNullException("dispatcherContext");
            }
            Context = dispatcherContext;
        }

        public EventType GetOneById(Guid eventTypeId, Guid accountId)
        {
            var repository = GetEventTypeRepository(accountId);
            var type = repository.GetById(eventTypeId);
            return type;
        }

        public EventType GetOrCreate(EventType eventType, Guid accountId)
        {
            var lockObj = LockObject.ForEventType(accountId, eventType.SystemName);
            lock (lockObj)
            {
                var repository = GetEventTypeRepository(accountId);
                return repository.GetOrCreate(eventType);
            }
        }

        public EventType GetOneBySystemName(Guid accountId, string systemName)
        {
            var repository = GetEventTypeRepository(accountId);
            var type = repository.GetOneOrNullBySystemName(systemName);
            if (type == null)
            {
                throw new Exception("Не удалось найти тип события " + systemName);
            }
            return type;
        }

        public void Update(Guid accountId, UpdateEventTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = GetEventTypeRepository(accountId);
            var eventType = repository.GetById(data.EventTypeId);

            if (eventType.IsSystem)
            {
                throw new UserFriendlyException("Нельзя менять системный тип события");
            }

            eventType.OldVersion = data.OldVersion;
            eventType.ImportanceForOld = data.ImportanceForOld;
            eventType.ImportanceForNew = data.ImportanceForNew;
            eventType.JoinIntervalSeconds = data.JoinIntervalSeconds;

            if (string.IsNullOrEmpty(data.DisplayName) == false)
            {
                eventType.DisplayName = data.DisplayName;
            }
            if (string.IsNullOrEmpty(data.SystemName) == false)
            {
                eventType.SystemName = data.SystemName;
            }

            accountDbContext.SaveChanges();

            if (data.UpdateActualEvents == false)
            {
                return;
            }

            // Обновим старые события
            long? oldVersion = VersionHelper.FromString(eventType.OldVersion);
            var now = DateTime.Now;
            var components = new List<Guid>();
            var eventRepository = accountDbContext.GetEventRepository();

            var events = eventRepository
                .GetAllByType(eventType.Id)
                .Where(t => t.ActualDate >= now) // неактуальные уже нина что не влияют, нет смысла их менять
                .ToList();

            // если есть событие - инициатор (на основании которого решили изменить важность)
            if (data.EventId.HasValue)
            {
                var eventObj = eventRepository.GetById(data.EventId.Value);
                events.Add(eventObj);
            }

            // обновим события в БД
            foreach (var _event in events)
            {
                if (eventType.ImportanceForOld.HasValue && _event.VersionLong <= oldVersion)
                {
                    // ставим важность для старых событий
                    if (_event.Importance != eventType.ImportanceForOld.Value)
                    {
                        _event.Importance = eventType.ImportanceForOld.Value;
                        components.Add(_event.OwnerId);
                    }
                }
                else if (eventType.ImportanceForNew.HasValue &&
                         (oldVersion == null || _event.VersionLong > oldVersion))
                {
                    // ставим важность для новых событий
                    if (_event.Importance != eventType.ImportanceForNew.Value)
                    {
                        _event.Importance = eventType.ImportanceForNew.Value;
                        components.Add(_event.OwnerId);
                    }
                }
            }
            accountDbContext.SaveChanges();

            // выгрузим события из кэша
            foreach (var _event in events)
            {
                AllCaches.Events.Unload(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = _event.Id
                });
            }

            // обновим статусы компонентов
            var componentIds = components.Distinct().ToList();
            foreach (var id in componentIds)
            {
                Context.ComponentService.CalculateEventsStatus(accountId, id);
            }

        }
    }
}
