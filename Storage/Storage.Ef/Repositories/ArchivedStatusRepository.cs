using System;
using System.Data.Entity;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class ArchivedStatusRepository : IArchivedStatusRepository
    {
        public ArchivedStatusRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(ArchivedStatusForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.ArchivedStatuses.Add(new DbArchivedStatus()
                {
                    EventId = entity.EventId
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Delete(long id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var archivedStatus = DbGetOneById(id);
                contextWrapper.Context.ArchivedStatuses.Remove(archivedStatus);
                contextWrapper.Context.SaveChanges();
            }
        }

        public ArchivedStatusGetForNotificationsInfo[] GetForNotifications(Guid? componentId, int maxCount)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.ArchivedStatuses.AsNoTracking()
                    .Include(t => t.Event);

                if (componentId.HasValue)
                    query = query.Where(t => t.Event.OwnerId == componentId.Value);

                return query.OrderBy(x => x.Id)
                    .Take(maxCount)
                    .Select(t => new ArchivedStatusGetForNotificationsInfo()
                    {
                        Id = t.Id,
                        Event = new ArchivedStatusGetForNotificationsInfo.EventInfo()
                        {
                            Id = t.Event.Id,
                            CreateDate = t.Event.CreateDate,
                            OwnerId = t.Event.OwnerId,
                            ActualDate = t.Event.ActualDate,
                            StartDate = t.Event.StartDate,
                            Category = t.Event.Category,
                            Importance = t.Event.Importance,
                            PreviousImportance = t.Event.PreviousImportance
                        }
                    }).ToArray();
            }
        }

        private DbArchivedStatus DbGetOneOrNullById(long id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.ArchivedStatuses.Find(id);
            }
        }

        private DbArchivedStatus DbGetOneById(long id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Архивный статус {id} не найден");

            return result;
        }
    }
}
