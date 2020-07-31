using System;
using System.Linq;
using Zidium.Common;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class EventCacheStorage : AccountDbCacheStorageBase<EventCacheResponse, IEventCacheReadObject, EventCacheWriteObject>
    {
        protected override EventCacheWriteObject LoadObject(AccountCacheRequest request, IStorage storage)
        {
            var eventObj = storage.Events.GetOneOrNullById(request.ObjectId);
            return EventCacheWriteObject.CreateForUpdate(eventObj, request.AccountId);
        }

        private void UpdateEntity(EventForAdd dbEvent, EventCacheWriteObject cacheEvent)
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
        }

        protected override void AddBatchObjects(IStorage storage, EventCacheWriteObject[] eventObjs)
        {
            var entities = eventObjs.Select(eventObj =>
            {
                if (eventObj.NewStatuses.Count > 0)
                {
                    throw new Exception("eventObj.NewStatuses.Count > 0");
                }

                var entity = new EventForAdd();
                UpdateEntity(entity, eventObj);
                return entity;
            }).ToArray();

            storage.Events.Add(entities);
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
        }

        protected override void UpdateBatchObjects(IStorage storage, EventCacheWriteObject[] eventObjs, bool useCheck)
        {
            var entities = eventObjs.Select(eventObj =>
            {
                var lastData = eventObj.Response.LastSavedData;

                if (lastData == null)
                {
                    throw new Exception("eventObj.Response.LastSavedData == null");
                }
                
                var entity = lastData.CreateEfEvent();

                if (lastData.Importance != eventObj.Importance)
                    entity.Importance.Set(eventObj.Importance);

                if (lastData.IsUserHandled != eventObj.IsUserHandled)
                    entity.IsUserHandled.Set(eventObj.IsUserHandled);

                if (lastData.LastNotificationDate != eventObj.LastNotificationDate)
                    entity.LastNotificationDate.Set(eventObj.LastNotificationDate);

                if (lastData.LastStatusEventId != eventObj.LastStatusEventId)
                    entity.LastStatusEventId.Set(eventObj.LastStatusEventId);

                if (lastData.LastUpdateDate != eventObj.LastUpdateDate)
                    entity.LastUpdateDate.Set(eventObj.LastUpdateDate);

                if (lastData.Message != eventObj.Message)
                    entity.Message.Set(eventObj.Message);

                if (lastData.StartDate != eventObj.StartDate)
                    entity.StartDate.Set(eventObj.StartDate);

                if (lastData.ActualDate != eventObj.ActualDate)
                    entity.ActualDate.Set(eventObj.ActualDate);

                if (lastData.Count != eventObj.Count)
                    entity.Count.Set(eventObj.Count);

                if (lastData.EndDate != eventObj.EndDate)
                    entity.EndDate.Set(eventObj.EndDate);

                if (lastData.FirstReasonEventId != eventObj.FirstReasonEventId)
                    entity.FirstReasonEventId.Set(eventObj.FirstReasonEventId);

                return entity;
            }).ToArray();

            var eventStatusEntities = eventObjs
                .SelectMany(t => t.NewStatuses.Select(x => new { EventObj = t, Status = x }))
                .Where(t => !t.Status.Saved).Select(z =>
                {
                    if (z.Status.StatusId == z.EventObj.Id)
                    {
                        throw new Exception("statusReference.StatusId == dbEvent.Id");
                    }

                    return new EventStatusForAdd()
                    {
                        EventId = z.EventObj.Id,
                        StatusId = z.Status.StatusId
                    };
                })
                .ToArray();

            using (var transaction = storage.BeginTransaction())
            {
                storage.Events.Update(entities);
                storage.Events.AddStatusToEvent(eventStatusEntities);
                transaction.Commit();
            }
        }

        public override int BatchCount
        {
            get { return 100; }
        }

        /*
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
        */

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

        public IEventCacheReadObject GetForRead(Guid eventId, Guid accountId)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = eventId
            };
            return Find(request);
        }

        public EventCacheWriteObject GetForWrite(Guid eventId, Guid accountId)
        {
            return Write(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = eventId
            });
        }
    }
}
