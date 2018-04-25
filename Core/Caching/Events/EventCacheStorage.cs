using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Caching
{
    public class EventCacheStorage : StorageDbCacheStorageBase<EventCacheResponse, IEventCacheReadObject, EventCacheWriteObject>
    {
        protected override EventCacheWriteObject LoadObject(AccountCacheRequest request, AccountDbContext accountDbContext)
        {
            var eventRepository = accountDbContext.GetEventRepository();
            var eventObj = eventRepository.GetByIdOrNull(request.ObjectId);
            return EventCacheWriteObject.CreateForUpdate(eventObj, request.AccountId);
        }

        protected void UpdateEntity(Event dbEvent, EventCacheWriteObject cacheEvent, bool useCheck)
        {
            dbEvent.Id = cacheEvent.Id;
            dbEvent.Importance = cacheEvent.Importance;
            dbEvent.IsSpace = cacheEvent.IsSpace;
            dbEvent.IsUserHandled = cacheEvent.IsUserHandled;
            dbEvent.JoinKeyHash = cacheEvent.JoinKeyHash;
            dbEvent.LastNotificationDate = cacheEvent.LastNotificationDate;
            dbEvent.LastStatusEventId = cacheEvent.LastStatusEventId;
            dbEvent.LastUpdateDate = cacheEvent.LastUpdateDate;
            dbEvent.Message = cacheEvent.Message;
            dbEvent.PreviousImportance = cacheEvent.PreviousImportance;
            dbEvent.OwnerId = cacheEvent.OwnerId;
            dbEvent.StartDate = cacheEvent.StartDate;
            dbEvent.Version = cacheEvent.Version;
            dbEvent.VersionLong = cacheEvent.VersionLong;
            dbEvent.ActualDate = cacheEvent.ActualDate;
            dbEvent.Category = cacheEvent.Category;
            dbEvent.Count = cacheEvent.Count;
            dbEvent.CreateDate = cacheEvent.CreateDate;
            dbEvent.EndDate = cacheEvent.EndDate;
            dbEvent.EventTypeId = cacheEvent.EventTypeId;
            dbEvent.FirstReasonEventId = cacheEvent.FirstReasonEventId;

            foreach (var property in cacheEvent.NewEventProperties)
            {
                // укажим ссылку на родителя явно, чтобы не было проблем при повторном сохранении свойства
                // когда данная ссылка уже проставлена на предыдущий объект, см. исключение ниже

               //System.InvalidOperationException: Conflicting changes to the role 'EventProperty_Event_Target' of the relationship 'Zidium.Core.StorageDb.EventProperty_Event' have been detected.
               //в System.Data.Entity.Core.Objects.DataClasses.EntityReference.AddToNavigationPropertyIfCompatible(RelatedEnd otherRelatedEnd)
               //в System.Data.Entity.Core.Objects.DataClasses.RelatedEnd.IncludeEntity(IEntityWrapper wrappedEntity, Boolean addRelationshipAsUnchanged, Boolean doAttach)
               //в System.Data.Entity.Core.Objects.DataClasses.EntityCollection`1.Include(Boolean addRelationshipAsUnchanged, Boolean doAttach)
               //в System.Data.Entity.Core.Objects.DataClasses.RelationshipManager.AddRelatedEntitiesToObjectStateManager(Boolean doAttach)
               //в System.Data.Entity.Core.Objects.ObjectContext.AddObject(String entitySetName, Object entity)
               //в System.Data.Entity.Internal.Linq.InternalSet`1.ActOnSet(Action action, EntityState newState, Object entity, String methodName)
               //в System.Data.Entity.Internal.Linq.InternalSet`1.Add(Object entity)
               //в System.Data.Entity.DbSet`1.Add(TEntity entity)
               //в Zidium.Core.StorageDb.EventRepository.Add(Event eventObj) в c:\Sasha\_Recursion\_MyProjects\appMonitoring\GIT\AppMonitoring\Core\StorageDb\Repositories\EventRepository.cs:строка 102
               //в Zidium.Core.Caching.EventCacheStorage.AddBatchObject(StorageDbContext storageDbContext, EventCacheWriteObject eventObj, Boolean checkAdd) в c:\Sasha\_Recursion\_MyProjects\appMonitoring\GIT\AppMonitoring\Core\Caching\Events\EventCacheStorage.cs:строка 68
               
                property.Event = dbEvent;
                property.EventId = dbEvent.Id;

                dbEvent.Properties.Add(property);
            }
        }

        protected override void AddBatchObject(AccountDbContext accountDbContext, EventCacheWriteObject eventObj, bool checkAdd)
        {
            if (eventObj.NewStatuses.Count > 0)
            {
                throw new Exception("eventObj.NewStatuses.Count > 0");
            }
            var repository = accountDbContext.GetEventRepository();
            if (checkAdd)
            {
                var fromDb = repository.GetByIdOrNull(eventObj.Id);
                if (fromDb != null)
                {
                    return;
                }
            }
            var dbEvent = new Event();
            UpdateEntity(dbEvent, eventObj, checkAdd);
            repository.Add(dbEvent);
        }

        protected override void SetResponseSaved(EventCacheWriteObject writeObject)
        {
            base.SetResponseSaved(writeObject);

            // после сохранения пометим сохраненные статусы
            foreach (var statusReference in writeObject.NewStatuses)
            {
                statusReference.Saved = true;
            }
            writeObject.UpdateNewStatuses();

            // снимаем флажок
            //writeObject.IsArchivedStatus = false;

            // очистим доп свойства
            writeObject.NewEventProperties.Clear();
        }

        protected override void UpdateBatchObject(
            AccountDbContext accountDbContext, 
            EventCacheWriteObject eventObj, 
            bool useCheck)
        {
            if (eventObj.Response.LastSavedData == null)
            {
                throw new Exception("eventObj.Response.LastSavedData == null");
            }
            var dbEvent = accountDbContext.Events.Local.FirstOrDefault(x => x.Id == eventObj.Id);
            if (dbEvent == null)
            {
                if (useCheck)
                {
                    // загрузим из БД, чтобы получить актуальные данные
                    var repository = accountDbContext.GetEventRepository();
                    dbEvent = repository.GetById(eventObj.Id);
                }
                else
                {
                    // создаем копию из кэша
                    dbEvent = eventObj.Response.LastSavedData.CreateEfEvent();
                    accountDbContext.Events.Attach(dbEvent);
                }
            }

            // обновляем свойства
            UpdateEntity(dbEvent, eventObj, useCheck);

            // сейчас архивный статус сохраняетя синхронно, когда создается новый статус лампочки

            // создаем архивный статус
            //if (eventObj.IsArchivedStatus)
            //{
            //    var archivedStatus = new ArchivedStatus()
            //    {
            //        EventId = eventObj.Id,
            //        AccountId = eventObj.AccountId
            //    };
            //    var archivedStatusRepository = storageDbContext.GetArchivedStatusRepository();
            //    archivedStatusRepository.Add(archivedStatus);
            //}

            // вставляем новые статусы
            foreach (var statusReference in eventObj.NewStatuses)
            {
                if (statusReference.Saved)
                {
                    continue;
                }
                if (statusReference.StatusId == dbEvent.Id)
                {
                    throw new Exception("statusReference.StatusId == dbEvent.Id");
                }
                Guid fakeStatusId = statusReference.StatusId;

                // проверим можно ли добавлять статус
                bool canAdd = true;
                if (useCheck)
                {
                    if (dbEvent.StatusEvents.Any(x => x.Id == fakeStatusId))
                    {
                        canAdd = false;
                    }
                }
                if (canAdd)
                {
                    var fakeStatus = accountDbContext.Events.Local.FirstOrDefault(x => x.Id == fakeStatusId);
                    if (fakeStatus == null)
                    {
                        if (useCheck)
                        {
                            var repository = accountDbContext.GetEventRepository();
                            fakeStatus = repository.GetByIdOrNull(fakeStatusId);
                            if (fakeStatus == null)
                            {
                                // значит статус удален
                                return;
                            }
                        }
                        else
                        {
                            fakeStatus = new Event() { Id = statusReference.StatusId };
                            accountDbContext.Events.Attach(fakeStatus);
                            accountDbContext.Entry(fakeStatus).State = EntityState.Unchanged;
                        }
                    }
                    dbEvent.StatusEvents.Add(fakeStatus);
                }
            }

            //var entity = storageDbContext.Entry(dbEvent);
            //var changed = GetChangedProperties(entity);
        }

        protected List<string> GetChangedProperties(DbEntityEntry<Event> eventEntity)
        {
            var changedList = new List<string>();
            foreach (var propertyName in eventEntity.OriginalValues.PropertyNames)
            {
                var original = eventEntity.OriginalValues[propertyName];
                var current = eventEntity.CurrentValues[propertyName];
                if (original == null && current == null)
                {
                    continue;
                }
                if (object.Equals(original, current))
                {
                    continue;
                }
                var changed = propertyName + ": " + original + " => " + current;
                changedList.Add(changed);
            }
            return changedList;
        }

        protected override Exception CreateNotFoundException(AccountCacheRequest request)
        {
            return new UserFriendlyException("Не удалось найти событие с ID " + request.ObjectId);
        }

        protected override void ValidateChanges(EventCacheWriteObject oldEvent, EventCacheWriteObject newEvent)
        {
            if (newEvent.Id == Guid.Empty)
            {
                throw new Exception("eventObj.Id == Guid.Empty");
            }
            if (newEvent.AccountId == Guid.Empty)
            {
                throw new Exception("eventObj.AccountId == Guid.Empty");
            }
            if (newEvent.EventTypeId == Guid.Empty)
            {
                throw new Exception("eventObj.EventTypeId == Guid.Empty");
            }
            if (newEvent.CreateDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.CreateDate == DateTime.MinValue");
            }
            if (newEvent.LastUpdateDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.LastUpdateDate == DateTime.MinValue");
            }
            if (newEvent.StartDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.StartDate == DateTime.MinValue");
            }
            if (newEvent.EndDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.EndDate == DateTime.MinValue");
            }
            if (newEvent.ActualDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.ActualDate == DateTime.MinValue");
            }
            //if (eventObj.Response.IsNewEntity && eventObj.NewStatuses.Count > 0)
            //{
            //    throw new Exception("eventObj.Response.IsNewEntity && eventObj.NewStatuses.Count > 0");
            //}
        }

        protected override bool HasChanges(EventCacheWriteObject oldObj, EventCacheWriteObject newObj)
        {
            if (newObj.NewStatuses.Count > 0)
            {
                return true;
            }
            return ObjectChangesHelper.HasChanges(oldObj, newObj);
        }

        public bool ExistsInStorage(IEventCacheReadObject eventObj)
        {
            if (eventObj == null)
            {
                throw new ArgumentNullException("eventObj");
            }
            var request = new AccountCacheRequest()
            {
                AccountId = eventObj.AccountId,
                ObjectId = eventObj.Id
            };
            return ExistsInStorage(request);
        }

        public IEventCacheReadObject GetForRead(Event eventObj, Guid accountId)
        {
            if (eventObj == null)
            {
                throw new ArgumentNullException("eventObj");
            }
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = eventObj.Id
            };
            return Find(request);
        }

        public EventCacheWriteObject GetForWrite(Event eventObj, Guid accountId)
        {
            if (eventObj == null)
            {
                throw new ArgumentNullException("eventObj");
            }
            return Write(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = eventObj.Id
            });
        }
    }
}
