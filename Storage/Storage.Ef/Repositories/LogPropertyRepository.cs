using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class LogPropertyRepository : ILogPropertyRepository
    {
        public LogPropertyRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public LogPropertyForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public LogPropertyForRead[] GetByLogId(Guid logId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.LogProperties.AsNoTracking()
                    .Where(t => t.LogId == logId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbLogProperty DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.LogProperties.Find(id);
            }
        }

        private DbLogProperty DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Свойство {id} не найдено");

            return result;
        }

        private LogPropertyForRead DbToEntity(DbLogProperty entity)
        {
            if (entity == null)
                return null;

            return new LogPropertyForRead(entity.Id, entity.LogId, entity.Name, entity.DataType, entity.Value);
        }
    }
}
