using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class EventPropertyRepository : IEventPropertyRepository
    {
        public EventPropertyRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public EventPropertyForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public EventPropertyForRead[] GetByEventId(Guid eventId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.EventProperties.AsNoTracking()
                    .Where(t => t.EventId == eventId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public EventPropertyForRead[] GetByEventIds(Guid[] eventIds)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.EventProperties.AsNoTracking()
                    .Where(t => eventIds.Contains(t.EventId))
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbEventProperty DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.EventProperties.Find(id);
            }
        }

        private DbEventProperty DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Свойство {id} не найдено");

            return result;
        }

        private EventPropertyForRead DbToEntity(DbEventProperty entity)
        {
            if (entity == null)
                return null;

            return new EventPropertyForRead(entity.Id, entity.EventId, entity.Name, entity.Value, entity.DataType);
        }
    }
}
